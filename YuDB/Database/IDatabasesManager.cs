namespace YuDB.Database
{
    public interface IDatabasesManager
    {
        void CreateDatabase(string databaseName);
        void CreateCollection(string databaseName, string collectionName, string schema);
        void WriteDocument(
            string databaseName,
            string collectionName,
            string content
        );
        void ReplaceDocument(
            string databaseName,
            string collectionName,
            string documentID,
            string updatedDocument
        );
        void DeleteDocument(
            string databaseName,
            string collectionName,
            string documentID
        );
        string ReadDocument(
            string databaseName,
            string collectionName,
            string documentID
        );
    }
}
