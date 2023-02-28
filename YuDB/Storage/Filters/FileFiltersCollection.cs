namespace YuDB.Storage.Filters
{
    public class FileFiltersCollection : IFileFilter
    {
        private List<IFileFilter> filters = new List<IFileFilter>();
        public FileFiltersCollection(params IFileFilter[] filters)
        {
            this.filters.AddRange(filters);
        }
        public byte[]? Do(byte[] data)
        {
            foreach (var filter in filters)
            {
                data = filter.Do(data);
            }
            return data;
        }

        public byte[]? Undo(byte[] data)
        {
            foreach (var filter in filters.Reverse<IFileFilter>())
            {
                data = filter.Undo(data);
            }
            return data;
        }
    }
}
