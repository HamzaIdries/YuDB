﻿using System.Security.Cryptography;

namespace YuDB.Storage.Filters
{
    /// <summary>
    /// Encrypt a document using AES
    /// </summary>
    public class FileEncryptionFilter : AbstractFileFilter, IDisposable
    {
        private readonly Aes _aes;

        private readonly byte[] _IV;

        public FileEncryptionFilter(byte[] token, byte[] IV)
        {
            _IV = IV;
            _aes = Aes.Create();
            _aes.Key = token;
        }

        public void Dispose()
        {
            _aes.Dispose();
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
                return _aes.EncryptCbc(data, _IV, PaddingMode.PKCS7);
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
                return _aes.DecryptCbc(data, _IV, PaddingMode.PKCS7);
            }
            catch (Exception ex)
            {
                throw new FileFilterException($"Couldn't decrypt document due to the following error: {ex.Message}");
            }
        }
    }
}