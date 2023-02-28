using System.Security.Cryptography;

namespace YuDB.Storage.Filters
{
    public class EncryptionFilter : IFileFilter, IDisposable
    {
        private Aes aes;
        private byte[] iv;
        public EncryptionFilter(byte[] key, byte[] iv)
        {
            aes = Aes.Create();
            aes.Key = key;
            this.iv = iv;
        }

        public void Dispose()
        {
            aes.Dispose();
        }

        public byte[]? Do(byte[] data)
        {
            return aes.EncryptCbc(data, iv, PaddingMode.PKCS7);
        }

        public byte[]? Undo(byte[] data)
        {
            return aes.DecryptCbc(data, iv, PaddingMode.PKCS7);
        }
    }
}
