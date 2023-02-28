using System.Text.Json;

namespace YuDB
{
    public class Config
    {
        private readonly string databasesDirectory;
        private readonly string passwordFilePath;
        private readonly string rootVariableName;
        private readonly int port;
        private readonly int queueSize;
        private readonly int maxRequestLength;
        public string DatabasesDirectory => databasesDirectory;
        public string PasswordFilePath => passwordFilePath;
        public string RootVariableName => rootVariableName;
        public int Port => port;
        public int QueueSize => queueSize;
        public int MaxRequestLength => maxRequestLength;

        private static Config? instance = null;
        public Config(
            string databasesDirectory, 
            string passwordFilePath,
            string rootVariableName, 
            int port,
            int queueSize,
            int maxRequestLength)
        {
            this.databasesDirectory = databasesDirectory;
            this.passwordFilePath = passwordFilePath;
            this.rootVariableName = rootVariableName;
            this.port = port;
            this.queueSize = queueSize;
            this.maxRequestLength = maxRequestLength;
        }
        public static Config Get()
        {
            if (instance == null)
            {
                var configurationFile = File.ReadAllText("./config.json");
                instance = JsonSerializer.Deserialize<Config>(configurationFile, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            return instance;
        }
    }
}
