using System.Security.Cryptography;

namespace YuDB.Storage.Filters
{
    public class IntegrityFilter : IFileFilter
    {
        private readonly byte[] key;
        public IntegrityFilter(byte[] key)
        {
            this.key = key;
        }
        public byte[]? Do(byte[] data)
        {
            var hash = HMACSHA256.HashData(key, data);
            byte[] file = new byte[hash.Length + data.Length];
            Array.Copy(hash, file, hash.Length);
            Array.Copy(data, 0, file, hash.Length, data.Length);
            return file;
        }
        public byte[]? Undo(byte[] file)
        {
            var actualHash = file.Take(32).ToArray();
            var data = file.Skip(32).ToArray();
            var expectedHash = HMACSHA256.HashData(key, data);
            return expectedHash.SequenceEqual(actualHash)
                ? data
                : null;
        }
    }
}
