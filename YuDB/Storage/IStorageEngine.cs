namespace YuDB.Storage
{
    public interface IStorageEngine
    {
        void Store(string documentPath, byte[] document);
        byte[] ReadDocument(string documentPath);
        IEnumerable<byte[]> ReadAllDocuments(string directory);
        void Replace(string documentPath, byte[] document);
        void Delete(string documentPath);
    }
}
