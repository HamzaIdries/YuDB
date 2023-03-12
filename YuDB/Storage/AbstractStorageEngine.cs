namespace YuDB.Storage
{
    /// <summary>
    /// Manages everything regarding storing, reading, updating and deleting documents
    /// </summary>
    public abstract class AbstractStorageEngine
    {
        /// <summary>
        /// Deletes a document in the given path
        /// </summary>
        public abstract void Delete(string documentPath);

        /// <summary>
        /// Reads a document from the given path
        /// </summary>
        public abstract byte[] Read(string documentPath);

        /// <summary>
        /// Replaces a document in the given path with the updated document
        /// </summary>
        public abstract void Replace(string documentPath, byte[] document);

        /// <summary>
        /// Stores a document in the given path
        /// </summary>
        public abstract void Store(string documentPath, byte[] document);
    }
}