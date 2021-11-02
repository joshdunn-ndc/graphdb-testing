using GraphDB_Testing.Gremlin;
using GraphDB_Testing.Power;
using GraphDB_Testing.Power.BulkDataManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;

namespace GraphDB_Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();

            serviceCollection.Configure<PowerOptions>(configuration.GetSection("PowerOptions"));
            serviceCollection.AddSingleton<IBulkDataManager, BulkDataManager>();

            var services = serviceCollection.BuildServiceProvider();
            var manager = services.GetService<IBulkDataManager>();
            (List<GremlinEdge> edges, List<GremlinVertex> vertices) gremlinData;

            gremlinData = SetupGremlinData();

            manager.ImportDataModelAsync().Wait();
            //manager.ImportSpreadsheetData(gremlinData.edges, gremlinData.vertices).Wait();
        }

        private static (List<GremlinEdge> edges, List<GremlinVertex> vertices) SetupGremlinData()
        {
            List<GremlinVertex> vertices;
            List<GremlinEdge> edges;


            var meep = new GremlinVertex("meep", "FACILITY");
            meep.AddProperty("description", "a useful description");
            meep.AddProperty("something", "data here");
            meep.AddProperty("location", "B1");
            meep.AddProperty("pk", "/pk");

            var beep = new GremlinVertex("beep", "FACILITY");
            beep.AddProperty("pk", "/pk");

            var edge = new GremlinEdge("edge1", "joins", "meep", "beep", "meep out", "beep in", "/pk", "/pk");

            vertices = new List<GremlinVertex> { meep, beep };
            edges = new List<GremlinEdge> { edge };

            return (edges, vertices);
        }
    }
}
