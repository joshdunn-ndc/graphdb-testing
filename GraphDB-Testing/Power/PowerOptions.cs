namespace GraphDB_Testing.Power
{
    /// <summary>
    /// Configuration options for the Power feature.
    /// </summary>
    public class PowerOptions
    {
        /// <summary>
        /// Section Name in appsettings.json.
        /// </summary>
        public static string Section => "PowerOptions";

        /// <summary>
        /// The authorization key to use.
        /// </summary>
        public string AuthKey { get; set; }

        /// <summary>
        /// The Azure endpoint to use.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The name of the database to use.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The name of the container to use.
        /// </summary>
        public string ContainerName { get; set; }
    }
}
