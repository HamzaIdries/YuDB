using System.Security.Cryptography;

namespace YuDB.Storage.Filters
{
    /// <summary>
    /// Ensures that document have not been illegally modified by checking its HMAC. The used
    /// hash function is SHA256
    /// </summary>
    public class FileIntegrityFilter : AbstractFileFilter
    {
        private readonly byte[] key;

        public FileIntegrityFilter(byte[] key)
        {
            this.key = key;
        }

        /// <summary>
        /// Appends an HMAC to a document
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override byte[] Do(byte[] data)
        {
            var hash = HMACSHA256.HashData(key, data);
            byte[] file = new byte[hash.Length + data.Length];
            Array.Copy(hash, file, hash.Length);
            Array.Copy(data, 0, file, hash.Length, data.Length);
            return file;
        }

        /// <summary>
        /// Removes the HMAC from the document and uses to verify that the document wasn't illegally modified.
        /// If it was illegally modified, an exception is thrown
        /// </summary>
        /// <exception cref="FileFilterException"></exception>
        public override byte[] Undo(byte[] file)
        {
            var actualHash = file.Take(32).ToArray();
            var data = file.Skip(32).ToArray();
            var expectedHash = HMACSHA256.HashData(key, data);
            if (expectedHash.SequenceEqual(actualHash))
                return data;
            else throw new IntegrityFailException("Document was illegally modified");
        }
    }
}