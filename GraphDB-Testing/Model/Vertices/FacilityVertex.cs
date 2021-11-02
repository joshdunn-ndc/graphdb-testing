using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB_Testing.Model.Vertices
{
    /// <summary>
    /// Defines a vertex for a facility.
    /// </summary>
    public class FacilityVertex : IVertex
    {

        public FacilityVertex(string id, string name, string description)
        {
            this.Id = id;
            this.Label = "FACILITY";
            this.PartitionKey = "/pk";

            this.Description = new List<CosmosDescriptor>
            {
                new CosmosDescriptor(description)
            };

            this.Name = new List<CosmosDescriptor>
            {
                new CosmosDescriptor(name)
            };
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public string Label { get; }

        /// <inheritdoc/>
        public string PartitionKey { get; }

        [JsonProperty("name")]
        public List<CosmosDescriptor> Name { get; }

        [JsonProperty("description")]
        public List<CosmosDescriptor> Description { get; }
    }
}
