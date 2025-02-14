﻿namespace YuDB.Storage.Filters
{
    /// <summary>
    /// Applies an ordered list of filters in the given order
    /// </summary>
    public class FileFiltersCollection : AbstractFileFilter
    {
        private List<AbstractFileFilter> _filters = new List<AbstractFileFilter>();

        public FileFiltersCollection(params AbstractFileFilter[] filters)
        {
            _filters.AddRange(filters);
        }

        /// <summary>
        /// Appends a file filter
        /// </summary>
        public void Add(AbstractFileFilter fileFilter)
        {
            _filters.Add(fileFilter);
        }

        /// <summary>
        /// Applies the provided list of filters in order
        /// </summary>
        /// <exception cref="FileFilterException"></exception>
        public override byte[] Do(byte[] data)
        {
            foreach (var filter in _filters)
                data = filter.Do(data);
            return data;
        }

        /// <summary>
        /// Removes the provided list of filters in reverse order
        /// </summary>
        /// <exception cref="FileFilterException"></exception>
        public override byte[] Undo(byte[] data)
        {
            foreach (var filter in _filters.Reverse<AbstractFileFilter>())
                data = filter.Undo(data);
            return data;
        }
    }
}