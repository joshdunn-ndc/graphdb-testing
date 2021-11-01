using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GraphDB_Testing
{
    /// <summary>
    /// Adapted from: https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-migrate-from-bulk-executor-library#execute-operations-concurrently.
    /// </summary>
    /// <typeparam name="T">The type of the data in the operation.</typeparam>
    public class BulkOperations<T>
    {
        
        public readonly List<Task<OperationResponse<T>>> Tasks;
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperations{T}"/> class.
        /// </summary>
        /// <param name="operationCount">The number of operations that will be performed.</param>
        public BulkOperations(int operationCount)
        {
            this.Tasks = new List<Task<OperationResponse<T>>>(operationCount);
            this.stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Executes all the operations.
        /// </summary>
        /// <returns>Response data for the operations containing any successful and failed documents.</returns>
        public async Task<BulkOperationResponse<T>> ExecuteAsync()
        {
            await Task.WhenAll(this.Tasks);
            this.stopwatch.Stop();

            return new BulkOperationResponse<T>()
            {
                TotalTimeTaken = this.stopwatch.Elapsed,
                TotalRequestUnitsConsumed = this.Tasks.Sum(task => task.Result.RequestUnitsConsumed),
                SuccessfulDocuments = this.Tasks.Count(task => task.Result.IsSuccessful),
                Failures = this.Tasks.Where(task => !task.Result.IsSuccessful).Select(task => (task.Result.Item, task.Result.CosmosException)).ToList()
            };
        }
    }
}
