using System;
using System.Collections.Generic;

namespace GraphDB_Testing
{
    /// <summary>
    /// Adapted from: https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-migrate-from-bulk-executor-library#capture-statistics.
    /// </summary>
    /// <typeparam name="T">The type of the response.</typeparam>
    public class BulkOperationResponse<T>
    {

        /// <summary>
        /// Gets or sets the total time taken for the operation.
        /// </summary>
        public TimeSpan TotalTimeTaken { get; set; }

        /// <summary>
        /// Gets or sets the number of documents that succeeded.
        /// </summary>
        public int SuccessfulDocuments { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total RSU's consumed.
        /// </summary>
        public double TotalRequestUnitsConsumed { get; set; } = 0;

        /// <summary>
        /// Gets or sets the documents that failed.
        /// </summary>
        public IReadOnlyList<(T, Exception)> Failures { get; set; }
    }
}
