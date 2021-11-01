using GraphDB_Testing.Gremlin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphDB_Testing.Power.BulkDataManager
{
    public interface IBulkDataManager
    {
        /// <summary>
        /// Imports the given spreadsheet data to the graph database.
        /// </summary>
        /// <param name="edges">The edges to import.</param>
        /// <param name="vertices">The vertices to import.</param>
        /// <returns>The response data for the import action.</returns>
        Task<BulkImportResponse> ImportSpreadsheetData(List<GremlinEdge> edges, List<GremlinVertex> vertices);
    }
}