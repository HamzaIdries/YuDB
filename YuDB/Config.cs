using System.Text;
using System.Text.Json;

namespace YuDB
{
    /// <summary>
    /// Provides access to the application's configurations
    /// </summary>
    public class Config
    {
        private static ConfigFile CONFIG_FILE = JsonSerializer.Deserialize<ConfigFile>(
            File.ReadAllText("./config.json"),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })!;

        public static string DatabasesDirectory => CONFIG_FILE.DatabasesDirectory;
        public static bool StrictMode => CONFIG_FILE.StrictMode;
        public static string UnauthorisedModificationsPolicy => CONFIG_FILE.UnauthorisedModificationsPolicy;
        public static string BackupPath => CONFIG_FILE.BackupPath;
    }

    /// <summary>
    /// Contains all the application configurations that are to be read from config.json file
    /// </summary>
    public class ConfigFile
    {
        private readonly string _databasesDirectory;
        private readonly string _backupPath;
        private readonly bool _strictMode;
        private readonly string _unauthorisedModificationsPolicy;

        public ConfigFile(string databasesDirectory, bool strictMode, string unauthorisedModificationsPolicy, string backupPath)
        {
            _databasesDirectory = databasesDirectory;
            _strictMode = strictMode;
            _unauthorisedModificationsPolicy = unauthorisedModificationsPolicy;
            _backupPath = backupPath;
        }

        public string DatabasesDirectory => _databasesDirectory;
        public bool StrictMode => _strictMode;
        public string UnauthorisedModificationsPolicy => _unauthorisedModificationsPolicy;
        public string BackupPath => _backupPath;
    }
}