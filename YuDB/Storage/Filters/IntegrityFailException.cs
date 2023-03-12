namespace YuDB.Storage.Filters
{
    public class IntegrityFailException : DatabaseException
    {
        public IntegrityFailException(string message) : base(message)
        {
        }
    }
}