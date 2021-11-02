using Newtonsoft.Json;

namespace GraphDB_Testing.Model.Edges
{
    /// <summary>
    /// Defines an 'Edge' in a Cosmos Graph database.
    /// </summary>
    public class Edge
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="id">The ID of the edge.</param>
        /// <param name="label">The label for the edge.</param>
        /// <param name="outVertexId">The ID of the 'out' vertex.</param>
        /// <param name="inVertexId">The ID of the 'in' vertex.</param>
        /// <param name="outVertexLabel">The label for the 'out' vertex.</param>
        /// <param name="inVertexLabel">The label for the 'in' vertex.</param>
        public Edge(
            string id, 
            string label, 
            string outVertexId, 
            string inVertexId, 
            string outVertexLabel, 
            string inVertexLabel
        )
        {
            this.Id = id;
            this.Label = label;
            this.OutVertexId = outVertexId;
            this.InVertexId = inVertexId;
            this.OutVertexLabel = outVertexLabel;
            this.InVertexLabel = inVertexLabel;

            // TODO: These will need to be updated eventually to be dynamic.
            this.OutVertexPartitionKey = "/pk";
            this.InVertexPartitionKey = "/pk";
        }

        /// <summary>
        /// Gets the ID of the edge.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; }

        /// <summary>
        /// Gets the label for the edge.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; }

        /// <summary>
        /// Gets the ID for the 'in' vertex.
        /// </summary>
        [JsonProperty("_sink")]
        public string InVertexId { get; }

        /// <summary>
        /// Gets the ID for the 'out' vertex.
        /// </summary>
        [JsonProperty("_vertexId")]
        public string OutVertexId { get; }

        /// <summary>
        /// Gets the label for the 'in' vertex.
        /// </summary>
        [JsonProperty("_sinkLabel")]
        public string InVertexLabel { get; }

        /// <summary>
        /// Gets the label for the 'out' vertex.
        /// </summary>
        [JsonProperty("_vertexLabel")]
        public string OutVertexLabel { get; }

        /// <summary>
        /// Gets whether the object is an edge.
        /// </summary>
        [JsonProperty("_isEdge")]
        public bool IsEdge { get; } = true;

        /// <summary>
        /// Gets the partition key for the 'out' vertex.
        /// </summary>
        [JsonProperty("pk")]
        public string OutVertexPartitionKey { get; }

        /// <summary>
        /// Gets the partition key for the 'in' vertex.
        /// </summary>
        [JsonProperty("_sinkPartition")]
        public string InVertexPartitionKey { get; }

    }
}
