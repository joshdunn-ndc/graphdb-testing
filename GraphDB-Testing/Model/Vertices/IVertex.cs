using Newtonsoft.Json;

namespace GraphDB_Testing.Model.Vertices
{
    /// <summary>
    /// Defines the properties that a vertex requires to be valid.
    /// </summary>
    public interface IVertex
    {
        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("label")]
        public string Label { get; }

        [JsonProperty("pk")]
        public string PartitionKey { get; }
    }
}
