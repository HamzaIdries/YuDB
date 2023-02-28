using System.Reflection.Metadata;

namespace YuDB
{
    public class DirectoriesStructure
    {
        public static string GetDatabasesDirectory()
        {
            var databasesDirectory = Config.Get().DatabasesDirectory;
            Directory.CreateDirectory(databasesDirectory);
            return databasesDirectory;
        }
        public static string GetDatabasePath(string databaseName)
        {
            var databasePath = Path.Join(GetDatabasesDirectory(), databaseName);
            Directory.CreateDirectory(databasePath);
            return databasePath;
        }
        public static string GetCollectionPath(string databaseName, string collectionName)
        {
            var collectionPath = Path.Join(GetDatabasePath(databaseName), collectionName);
            Directory.CreateDirectory(collectionPath);
            return collectionPath;
        }
        public static string GetDocumentsPath(string databaseName, string collectionName)
        {
            var documentsPath = Path.Join(GetCollectionPath(databaseName, collectionName), "documents");
            Directory.CreateDirectory(documentsPath);
            return documentsPath;
        }
        public static string GetDocumentPath(string databaseName, string collectionName, string documentName)
        {
            var documentPath = Path.Join(GetDocumentsPath(databaseName, collectionName), string.Format($"{documentName}.json"));
            return documentPath;
        }
        public static string GetSchema(string databaseName, string collectionName)
        {
            return Path.Join(GetCollectionPath(databaseName, collectionName), "schema.json");
        }
    }
}
