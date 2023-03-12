using LazyCache;
using YuDB.Storage.Filters;

namespace YuDB.Storage
{
    /// <summary>
    /// Performs CRUD operations on documents, but utilizes a cache to improve performance, and
    /// applies given filters on every document
    /// </summary>
    public class FilteredCachedStorageEngine : AbstractStorageEngine
    {
        private readonly IAppCache _cache = new CachingService();

        private readonly AbstractFileFilter _filter;

        public FilteredCachedStorageEngine(AbstractFileFilter filter)
        {
            _filter = filter;
        }

        public override void Delete(string documentPath)
        {
            // Throw an exception if the document to be deleted doesn't exist
            if (!File.Exists(documentPath))
                throw new DatabaseException($"Cannot delete document at '{documentPath}' as the document doesn't exist");

            // Remove the document from the cache when it's deleted
            if (_cache.TryGetValue(documentPath, out Lazy<byte[]> _))
                _cache.Remove(documentPath);
            File.Delete(documentPath);
        }

        public override byte[] Read(string documentPath)
        {
            // Attempts to read a file from the cache, otherwise reads the file and removes the filter
            return _cache.GetOrAdd(
                    documentPath,
                    path => _filter.Undo(File.ReadAllBytes(documentPath)));
        }

        public override void Replace(string documentPath, byte[] document)
        {
            // Throw an exception if the document to be replaced doesn't exist
            if (!File.Exists(documentPath))
                throw new DatabaseException($"Cannot replace document at '{documentPath}' as the document doesn't exist");

            // Remove the old version of the document from the cache
            if (_cache.TryGetValue(documentPath, out Lazy<byte[]> _))
                _cache.Remove(documentPath);
            Delete(documentPath);
            Store(documentPath, document);
        }

        public override void Store(string documentPath, byte[] document)
        {
            // Throw exception if attempting to override an already existing document
            if (File.Exists(documentPath))
                throw new DatabaseException($"Cannot store the document at '{documentPath}' as there already exists a document there");

            // Stores a file after applying the filter
            File.WriteAllBytes(documentPath, _filter.Do(document));
        }
    }
}