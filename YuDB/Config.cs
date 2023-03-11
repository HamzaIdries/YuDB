using System.Collections.Immutable;
using System.Text.Json;

namespace YuDB
{
    /// <summary>
    /// Contains all the application configurations that are to be read from config.json file
    /// </summary>
    public class Config
    {
        private string? databasesDirectory;

        private bool strictMode;

        private string? unauthorisedModificationsPolicy;

        public string DatabasesDirectory
        {
            get
            {
                ArgumentNullException.ThrowIfNull(databasesDirectory, "DatabasesDirectory");
                return databasesDirectory;
            }
            set { databasesDirectory = value; }
        }

        public bool StrictMode
        {
            get => strictMode;
            set { strictMode = value; }
        }

        public string UnauthorisedModificationsPolicy
        {
            get
            {
                ArgumentNullException.ThrowIfNull(unauthorisedModificationsPolicy, "UnauthorisedModificationsPolicy");
                if (ImmutableArray.Create("ignore", "warning", "error").Contains(unauthorisedModificationsPolicy))
                    return unauthorisedModificationsPolicy;
                throw new DatabaseException($"{unauthorisedModificationsPolicy} is not a valid unauthorised modifications policy");
            }
            set { unauthorisedModificationsPolicy = value; }
        }

        private static Config? INSTANCE = null;

        public static Config Get()
        {
            if (INSTANCE == null)
            {
                try
                {
                    var configFile = File.ReadAllText("./config.json")!;
                    INSTANCE = JsonSerializer.Deserialize<Config>(
                        configFile,
                        new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        })!;
                }
                catch (Exception)
                {
                    throw new DatabaseException("An issue occurred while trying to read the config file");
                }
            }
            return INSTANCE;
        }
    }
}