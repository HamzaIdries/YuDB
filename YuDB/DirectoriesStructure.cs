namespace YuDB
{
    /// <summary>
    /// Describes the directoy structure of the entire application
    /// </summary>
    public class DirectoriesStructure
    {
        /// <summary>
        /// Retrieves the root directory that contains all the system's databases
        /// </summary>
        public static string GetDatabasesDirectory()
        {
            var databasesDirectory = Config.Get().DatabasesDirectory;
            return databasesDirectory;
        }

        /// <summary>
        /// Retrieves the path of a specific database
        /// </summary>
        public static string GetDatabasePath(string databaseName)
        {
            var databasePath = Path.Join(GetDatabasesDirectory(), databaseName);
            return databasePath;
        }

        /// <summary>
        /// Retrieves the path of a specific collection
        /// </summary>
        public static string GetCollectionPath(string databaseName, string collectionName)
        {
            var collectionPath = Path.Join(GetDatabasePath(databaseName), collectionName);
            return collectionPath;
        }

        /// <summary>
        /// Retrieves the path where documents are stored in a specific collection
        /// </summary>
        public static string GetDocumentsPath(string databaseName, string collectionName)
        {
            var documentsPath = Path.Join(GetCollectionPath(databaseName, collectionName), "documents");
            return documentsPath;
        }

        /// <summary>
        /// Retrieves the path of a specific document
        /// </summary>
        public static string GetDocumentPath(string databaseName, string collectionName, string documentName)
        {
            var documentPath = Path.Join(GetDocumentsPath(databaseName, collectionName), string.Format($"{documentName}.json"));
            return documentPath;
        }

        /// <summary>
        /// Retrieves the path where a specific collection's schema is stored
        /// </summary>
        public static string GetSchemaPath(string databaseName, string collectionName)
        {
            return Path.Join(GetCollectionPath(databaseName, collectionName), "schema.json");
        }

        public static string GetPasswordFilePath()
        {
            return $"./{Config.Get().DatabasesDirectory}/pwd.json";
        }
    }
}