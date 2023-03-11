namespace YuDB.Managers
{
    /// <summary>
    /// Manages the entire application, and connects the various components with each other
    /// </summary>
    public abstract class AbstractDatabasesManager
    {
        /// <summary>
        /// Creates a collection with the specified name and schema within the specified database
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        public abstract void CreateCollection(
            string databaseName,
            string collectionName,
            string schema);

        /// <summary>
        /// Creates a database with the specified name
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        public abstract void CreateDatabase(
            string databaseName);

        /// <summary>
        /// Deletes a collection with the specified name
        /// </summary>
        public abstract void DeleteCollection(
            string databaseName,
            string collectionName);

        /// <summary>
        /// Deletes a database with the specified name
        /// </summary>
        public abstract void DeleteDatabase(
            string databaseName);

        /// <summary>
        /// Deletes the specified document
        /// </summary>
        public abstract void DeleteDocument(
            string databaseName,
            string collectionName,
            string documentID);

        /// <summary>
        /// Deletes all documents that satisfy the given predicate
        /// </summary>
        /// <returns>The number of deleted documents</returns>
        public abstract int DeleteDocuments(
            string databaseName,
            string collectionName,
            Func<string, bool> predicate);

        /// <summary>
        /// Lists all the collections in a specific database
        /// </summary>
        public abstract List<string> ListCollections(string databaseName);

        /// <summary>
        /// Lists all the databases in the system
        /// </summary>
        public abstract List<string> ListDatabases();

        /// <summary>
        /// Reads all documents that satisfy the given predicate
        /// </summary>
        public abstract List<string> ReadDocuments(
            string databaseName,
            string collectionName,
            Func<string, bool> predicate);

        /// <summary>
        /// Updates all documents that satisfy the given predicate
        /// </summary>
        /// <returns>The number of replaced documents</returns>
        public abstract int ReplaceDocuments(
            string databaseName,
            string collectionName,
            Func<string, bool> predicate,
            Func<string, string> updater);

        /// <summary>
        /// Writes a document within the specified collection. The document must adhere to the
        /// specified collection's schema in order for the write to succeed
        /// </summary>
        public abstract void WriteDocument(
            string databaseName,
            string collectionName,
            string content);
    }
}