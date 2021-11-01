using GraphDB_Testing.Gremlin;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GraphDB_Testing.Power.BulkDataManager
{
    /// <summary>
    /// Defines a data manager for bulk database ingestion.
    /// </summary>
    public class BulkDataManager : IBulkDataManager
    {
        
        private readonly PowerOptions _options;
        private readonly string _endpoint;
        private readonly string _authKey;
        private readonly string _databaseName;
        private readonly string _containerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDataManager"/> class.
        /// </summary>
        /// <param name="options">The options to use.</param>
        public BulkDataManager(IOptions<PowerOptions> options)
        {
            _options = options.Value;
            _endpoint = _options.Endpoint;
            _authKey = _options.AuthKey;
            _databaseName = _options.DatabaseName;
            _containerName = _options.ContainerName;
        }


        /// <inheritdoc/>
        public async Task<BulkImportResponse> ImportSpreadsheetData(
            List<GremlinEdge> edges, 
            List<GremlinVertex> vertices
        )
        {
            CosmosClient client;
            Database database;
            Container container;
            BulkOperations<ResponseMessage> bulkOperations;
            BulkOperationResponse<ResponseMessage> response;


            client = new CosmosClient(_endpoint, _authKey, new CosmosClientOptions() { AllowBulkExecution = true });
            database = await client.CreateDatabaseIfNotExistsAsync(_databaseName);
            container = await database.CreateContainerIfNotExistsAsync(_containerName, "/pk");

            bulkOperations = new BulkOperations<ResponseMessage>(vertices.Count + edges.Count);

            foreach (GremlinVertex data in vertices)
            {
                RecordCreateAction(
                    Helpers.GetVertexDocumentString(
                        data,
                        "pk",
                        false,
                        false
                    ),
                    container,
                    bulkOperations
                );
            }

            foreach (GremlinEdge data in edges)
            {
                RecordCreateAction(
                    Helpers.GetEdgeDocumentString(
                        data,
                        true,
                        "pk",
                        new HashSet<string>()
                    ),
                    container,
                    bulkOperations
                );
            }

            response = await bulkOperations.ExecuteAsync();

            if (response == null)
            {
                // TODO: Should we create a custom exception for this?
                throw new ApplicationException();
            }

            // TODO: Determine the data that we actually need to return.
            return new BulkImportResponse();
        }


        /// <summary>
        /// Adds the given JSON data to the bulk operations.
        /// </summary>
        /// <param name="jsonData">The JSON data to add.</param>
        /// <param name="container">The container to add it to.</param>
        /// <param name="operations">The bulk operations collection to add the action to.</param>
        private void RecordCreateAction(
            string jsonData,
            Container container,
            BulkOperations<ResponseMessage> operations
        )
        {

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(jsonData);
                    writer.Flush();
                    stream.Position = 0;

                    operations.Tasks.Add(
                        CaptureOperationResponse(
                            container.CreateItemStreamAsync(stream, new PartitionKey("/pk")),
                            new ResponseMessage()
                        )
                    );
                }

            }
        }


        /// <summary>
        /// Adapted from: https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-migrate-from-bulk-executor-library#capture-task-result-state
        /// </summary>
        /// <typeparam name="T">The type of the operation.</typeparam>
        /// <param name="task">The task to execute.</param>
        /// <param name="item">The item to record.</param>
        /// <returns>The response data.</returns>
        private async Task<OperationResponse<T>> CaptureOperationResponse<T>(
            Task<ResponseMessage> task, 
            T item
        )
        {
            try
            {
                ResponseMessage response = await task;
                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessful = true,
                };
            }
            catch (Exception ex)
            {
                if (ex is CosmosException cosmosException)
                {
                    return new OperationResponse<T>()
                    {
                        Item = item,
                        RequestUnitsConsumed = cosmosException.RequestCharge,
                        IsSuccessful = false,
                        CosmosException = cosmosException
                    };
                }

                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessful = false,
                    CosmosException = ex
                };
            }
        }
    }
}
