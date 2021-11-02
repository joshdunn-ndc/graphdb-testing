using GraphDB_Testing.Gremlin;
using GraphDB_Testing.Model.Edges;
using GraphDB_Testing.Model.Vertices;
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
            _endpoint = options.Value.Endpoint;
            _authKey = options.Value.AuthKey;
            _databaseName = options.Value.DatabaseName;
            _containerName = options.Value.ContainerName;
        }

        public async Task<BulkOperationResponse<object>> ImportDataModelAsync()
        {
            CosmosClient client;
            Database database;
            Container container;
            BulkOperations<object> bulkOperations;
            BulkOperationResponse<object> response;
            List<IVertex> vertices;
            List<Edge> edges;


            client = new CosmosClient(_endpoint, _authKey, new CosmosClientOptions() { AllowBulkExecution = true });
            database = await client.CreateDatabaseIfNotExistsAsync(_databaseName);
            container = await database.CreateContainerIfNotExistsAsync(_containerName, "/pk");

            vertices = new List<IVertex>
            {
                new FloorVertex("First", "First Vertex", "Description goes here I guess"),
                new FloorVertex("Second", "Second Vertex", "Another description."),
                new FacilityVertex("B1", "Brisbane 1", "Brisbane Generation 1")
            };

            edges = new List<Edge>
            {
                new Edge("FirstEdge", "Edge Label", "Second", "First", "Out Label", "In Label"),
                new Edge("Facility Edge", "Facility To Second", "B1", "Second", "Out Label", "In Label")
            };

            bulkOperations = new BulkOperations<object>(vertices.Count + edges.Count);

            foreach (IVertex data in vertices)
            {
                RecordCreateItemAction(
                    data,
                    container,
                    bulkOperations
                );
            }

            foreach (Edge data in edges)
            {
                RecordCreateItemAction(
                    data,
                    container,
                    bulkOperations
                );
            }

            response = await bulkOperations.ExecuteAsync();

            if (response.Failures.Count > 0)
            {
                // TODO: Remove this block and just return the above method.
                // The response can have both failures and successful documents so
                // we just need to return that instead of throwing an exception.
                throw new ApplicationException();
            }

            return response;
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
                RecordCreateItemStreamAction(
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
                RecordCreateItemStreamAction(
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
        private void RecordCreateItemStreamAction(
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
                        CaptureStreamOperationResponse(
                            container.CreateItemStreamAsync(stream, new PartitionKey("/pk")),
                            new ResponseMessage()
                        )
                    );
                }

            }
        }


        private void RecordCreateItemAction(
            object data,
            Container container,
            BulkOperations<object> operations
        )
        {

            // TODO: Change the PK creation to actually use the 'pk' property off the object.
            operations.Tasks.Add(
                CaptureOperationResponse(
                    container.CreateItemAsync(
                        data, 
                        new PartitionKey("/pk")
                    ),
                    data
                )
            );
        }


        private static async Task<OperationResponse<T>> CaptureOperationResponse<T>(Task<ItemResponse<T>> task, T item)
        {
            try
            {
                ItemResponse<T> response = await task;
                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessful = true,
                    RequestUnitsConsumed = task.Result.RequestCharge
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



        /// <summary>
        /// Adapted from: https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-migrate-from-bulk-executor-library#capture-task-result-state
        /// </summary>
        /// <typeparam name="T">The type of the operation.</typeparam>
        /// <param name="task">The task to execute.</param>
        /// <param name="item">The item to record.</param>
        /// <returns>The response data.</returns>
        private async Task<OperationResponse<T>> CaptureStreamOperationResponse<T>(
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
