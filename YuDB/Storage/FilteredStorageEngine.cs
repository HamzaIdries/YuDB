using LazyCache;
using System.Text;
using YuDB.Storage.Filters;

namespace YuDB.Storage
{
    public class FilteredStorageEngine : IStorageEngine
    {
        private readonly IAppCache cache = new CachingService();
        private readonly IFileFilter filter;
        private static FilteredStorageEngine? INSTANCE = null;
        public static FilteredStorageEngine Create(IFileFilter filter)
        {
            if (INSTANCE == null)
                INSTANCE = new FilteredStorageEngine(filter);
            return INSTANCE;
        }
        private FilteredStorageEngine(IFileFilter filter)
        {
            this.filter = filter;
        }
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
            File.WriteAllBytes(documentPath, filter.Do(document));
        }

        public byte[] ReadDocument(string documentPath)
        {
            var document = cache.GetOrAdd(
                documentPath,
                path => filter.Undo(File.ReadAllBytes(documentPath))
            );
            return document;
        }
    }
}
