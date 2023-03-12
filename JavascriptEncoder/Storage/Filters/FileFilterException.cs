namespace YuDB.Storage.Filters
{
    /// <summary>
    /// Thrown when an instance of IFileFilter failes
    /// </summary>
    /// <see cref="AbstractFileFilter"/>
    internal class FileFilterException : Exception
    {
        public FileFilterException(string message) : base(message)
        {
        }
    }
}