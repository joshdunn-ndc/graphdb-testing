using Newtonsoft.Json;
using System;

namespace GraphDB_Testing.Model.Vertices
{
    /// <summary>
    /// Defines a class which is able to serialize custom properties on a vertex.
    /// </summary>
    public class CosmosDescriptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDescriptor"/> class.
        /// </summary>
        /// <param name="value">The value to store.</param>
        public CosmosDescriptor(string value)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Value = value;
        }

        /// <summary>
        /// Gets the ID of the descriptor.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; }

        /// <summary>
        /// Gets the value of the descriptor.
        /// </summary>
        [JsonProperty("_value")]
        public string Value { get; }
    }
}
