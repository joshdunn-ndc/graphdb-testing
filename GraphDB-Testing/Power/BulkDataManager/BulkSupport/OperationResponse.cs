using System;

namespace GraphDB_Testing
{
    /// <summary>
    /// Adapted from: https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-migrate-from-bulk-executor-library#capture-task-result-state.
    /// </summary>
    /// <typeparam name="T">The type of the data in the operation.</typeparam>
    public class OperationResponse<T>
    {

        /// <summary>
        /// Gets or sets the item to use in the operation.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// Gets or sets the number of request units consumed by this operation.
        /// </summary>
        public double RequestUnitsConsumed { get; set; } = 0;

        /// <summary>
        /// Gets or sets whether the operation is successful.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the exception thrown by Cosmos, if any.
        /// </summary>
        public Exception CosmosException { get; set; }
    }
}
