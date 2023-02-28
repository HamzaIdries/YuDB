namespace YuDB.Storage.Filters
{
    public interface IFileFilter
    {
        byte[]? Do(byte[] data);
        byte[]? Undo(byte[] data);
    }
}
