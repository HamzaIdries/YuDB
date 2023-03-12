using System.Security.Cryptography;
using System.Text;

namespace YuDB.Storage.Filters
{
    /// <summary>
    /// Encrypt a document using AES
    /// </summary>
    public class FileEncryptionFilter : AbstractFileFilter, IDisposable
    {
        private readonly Aes aes;

        private readonly byte[] IV;

        public FileEncryptionFilter(byte[] token, byte[] IV)
        {
            this.IV = IV;
            aes = Aes.Create();
            aes.Key = token;
        }

        public void Dispose()
        {
            aes.Dispose();
        }

        /// <summary>
        /// Encrypts a document using AES
        /// </summary>
        /// <returns>The encrypted document</returns>
        /// <exception cref="FileFilterException"></exception>
        public override byte[] Do(byte[] data)
        {
            try
            {
                return aes.EncryptCbc(data, IV, PaddingMode.PKCS7);
            }
            catch (Exception ex)
            {
                throw new FileFilterException($"Couldn't encrypt document due to the following error: {ex.Message}");
            }
        }

        /// <summary>
        /// Decrypts an AES encrypted document
        /// </summary>
        /// <returns>The decrypted document</returns>
        /// <exception cref="FileFilterException"></exception>
        public override byte[] Undo(byte[] data)
        {
            try
            {
                return aes.DecryptCbc(data, IV, PaddingMode.PKCS7);
            }
            catch (Exception ex)
            {
                throw new FileFilterException($"Couldn't decrypt document due to the following error: {ex.Message}");
            }
        }
    }
}