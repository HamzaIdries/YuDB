using LazyCache;
using System.Text;

namespace YuDB.Storage
{
    public class StorageEngine : IStorageEngine
    {
        private readonly IAppCache cache = new CachingService();
        private static readonly StorageEngine INSTANCE = new StorageEngine();
        public static StorageEngine Create()
        {
            return INSTANCE;
        }
        private StorageEngine() { }
        public void Delete(string documentPath)
        {
            if (cache.TryGetValue(documentPath, out Lazy<string> _))
                cache.Remove(documentPath);
            if (File.Exists(documentPath))
                File.Delete(documentPath);
            // TODO: Error
        }

        public IEnumerable<byte[]> ReadAllDocuments(string directory)
        {
            foreach (string filepath in Directory.EnumerateFiles(directory))
            {
                var document = ReadDocument(filepath);
                if (document != null)
                    yield return document;
            }
        }

        public void Replace(string documentPath, byte[] document)
        {
            if (cache.TryGetValue(documentPath, out Lazy<string> _))
                cache.Remove(documentPath);
            if (File.Exists(documentPath))
            {
                Delete(documentPath);
                Store(documentPath, document);
            }
            else
            {
                // TODO: Error handling
            }
        }

        public void Store(string documentPath, byte[] document)
        {
            File.WriteAllBytes(documentPath, document);
        }

        public byte[] ReadDocument(string documentPath)
        {
            var document = cache.GetOrAdd(
                documentPath,
                path => File.ReadAllBytes(documentPath)
            );
            return document;
        }
    }
}
