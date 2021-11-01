using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GraphDB_Testing.Model.Vertices
{
    class FloorVertex
    {

        public FloorVertex(string id, string name, string description)
        {
            this.id = id;
            label = "FLOOR";
            pk = "/pk";
            type = "vertex";
            this.name = name;
            this.description = new List<Description>
            {
                new Description { value = description, id = Guid.NewGuid().ToString() }
            };
        }

        public Properties properties { get; }

        [JsonProperty("_type")]
        public string type { get; }

        public string id { get; }

        public string label { get; }

        public string pk { get; }

        public string name { get; }

        public List<Description> description { get; }

        public int operationCounter { get; set; } = 0;


    }

    class Properties
    {
        public List<Description> description { get; set; }
    }

    class Description
    {
        public string id { get; set; }

        [JsonProperty("_value")]
        public string value { get; set; }
    }


}
