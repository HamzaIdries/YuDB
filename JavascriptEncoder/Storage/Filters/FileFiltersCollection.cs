namespace YuDB.Storage.Filters
{
    /// <summary>
    /// Applies an ordered list of filters in the given order
    /// </summary>
    public class FileFiltersCollection : AbstractFileFilter
    {
        private List<AbstractFileFilter> filters = new List<AbstractFileFilter>();

        public FileFiltersCollection(params AbstractFileFilter[] filters)
        {
            this.filters.AddRange(filters);
        }

        /// <summary>
        /// Appends a file filter
        /// </summary>
        public void Add(AbstractFileFilter fileFilter)
        {
            filters.Add(fileFilter);
        }

        /// <summary>
        /// Applies the provided list of filters in order
        /// </summary>
        /// <exception cref="FileFilterException"></exception>
        public override byte[] Do(byte[] data)
        {
            foreach (var filter in filters)
                data = filter.Do(data);
            return data;
        }

        /// <summary>
        /// Removes the provided list of filters in reverse order
        /// </summary>
        /// <exception cref="FileFilterException"></exception>
        public override byte[] Undo(byte[] data)
        {
            foreach (var filter in filters.Reverse<AbstractFileFilter>())
                data = filter.Undo(data);
            return data;
        }
    }
}