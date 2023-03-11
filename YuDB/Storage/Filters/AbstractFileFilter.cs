namespace YuDB.Storage.Filters
{
    /// <summary>
    /// Describes a filter that takes in a document as a byte array and performs an
    /// implementation-dependent reversible operation on it
    /// </summary>
    public abstract class AbstractFileFilter
    {
        /// <summary>
        /// Performs a reversible operation on a document
        /// </summary>
        /// <returns>The document after applying the operation on it</returns>
        /// <exception cref="FileFilterException"></exception>
        public abstract byte[] Do(byte[] data);

        /// <summary>
        /// Reverses the operation done on a document
        /// </summary>
        /// <returns>The document prior to applying the operation</returns>
        /// <exception cref="FileFilterException"></exception>
        public abstract byte[] Undo(byte[] data);
    }
}