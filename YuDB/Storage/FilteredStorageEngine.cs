using YuDB.Storage.Filters;

namespace YuDB.Storage
{
    /// <summary>
    /// Performs CRUD operations on documents, but does not utilizes a cache to improve performance, and
    /// applies given filters on every document. Used only for benchmark purposes.
    /// </summary>
    public class FilteredStorageEngine : AbstractStorageEngine
    {
        private readonly AbstractFileFilter _filter;

        public FilteredStorageEngine(AbstractFileFilter filter)
        {
            _filter = filter;
        }

        public override void Delete(string documentPath)
        {
            // Throw an exception if the document to be deleted doesn't exist
            if (!File.Exists(documentPath))
                throw new DatabaseException($"Cannot delete document at '{documentPath}' as the document doesn't exist");
            File.Delete(documentPath);
        }

        public override byte[] Read(string documentPath)
        {
            // Attempts to read a file from the cache, otherwise reads the file and removes the filter
            return _filter.Undo(File.ReadAllBytes(documentPath));
        }

        public override void Replace(string documentPath, byte[] document)
        {
            // Throw an exception if the document to be replaced doesn't exist
            if (!File.Exists(documentPath))
                throw new DatabaseException($"Cannot replace document at '{documentPath}' as the document doesn't exist");
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