using Newtonsoft.Json;
using System.Collections.Generic;

namespace GraphDB_Testing.Model.Vertices
{
    public class FloorVertex : IVertex
    {

        public FloorVertex(string id, string name, string description)
        {
            this.Id = id;
            Label = "FLOOR";
            PartitionKey = "/pk";
            this.name = name;
            this.description = new List<CosmosDescriptor>
            {
                new CosmosDescriptor(description)
            };

            this.location = new List<CosmosDescriptor>
            {
                new CosmosDescriptor("Location place")
            };

            this.facility = new CosmosDescriptor("B1");

            this.properties = new Dictionary<string, List<CosmosDescriptor>>
            {
                {"Test", new List<CosmosDescriptor>{new CosmosDescriptor("thing here")} }
            };

        }

        public string Id { get; }

        public string Label { get; }

        public string PartitionKey { get; }

        public string name { get; }

        [JsonProperty("_properties")]
        public Dictionary<string, List<CosmosDescriptor>> properties;

        public CosmosDescriptor facility { get; }

        public List<CosmosDescriptor> location { get; }

        public List<CosmosDescriptor> description { get; }

        public int operationCounter { get; set; } = 0;
    }
}
