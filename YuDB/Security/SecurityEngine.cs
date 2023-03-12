using Autofac.Features.AttributeFilters;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using YuDB.Storage;

namespace YuDB.Security
{
    public class SecurityEngine : AbstractSecurityEngine
    {
        private readonly AbstractStorageEngine _storageEngine;

        public SecurityEngine([KeyFilter("defaultStorageEngine")] AbstractStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        public override bool Authenticate(Credentials credentials)
        {
            var passwordFile = GetPasswordFile();
            return
                credentials.Username == passwordFile.Username &&
                HashPassword(credentials.Password, passwordFile.Salt) == passwordFile.HashedPassword;
        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            var oldPasswordFile = GetPasswordFile();
            var credentials = new Credentials(oldPasswordFile.Username, oldPassword);
            if (!Authenticate(credentials))
                return false;
            var salt = GenerateSalt();
            var token = Convert.ToBase64String(GetToken(oldPassword));
            var newPasswordFile = new PasswordFile(
                oldPasswordFile.Username,
                HashPassword(newPassword, salt),
                salt,
                EncryptToken(newPassword, token));
            StorePasswordFile(newPasswordFile);
            return true;
        }

        public override bool ChangeUsername(string username)
        {
            var oldPasswordFile = GetPasswordFile();
            var newPasswordFile = new PasswordFile(
                username,
                oldPasswordFile.HashedPassword,
                oldPasswordFile.Salt,
                oldPasswordFile.EncryptedToken);
            StorePasswordFile(newPasswordFile);
            return true;
        }

        public override void CreateUser(Credentials credentials)
        {
            var salt = GenerateSalt();
            var token = GenerateToken();
            var passwordFile = new PasswordFile(
                credentials.Username,
                HashPassword(credentials.Password, salt),
                salt,
                EncryptToken(credentials.Password, token));
            StorePasswordFile(passwordFile);
        }

        public override byte[] GetToken(string password)
        {
            using var aes = Aes.Create();
            aes.Key = GetAESKeyFromPassword(password);
            var passwordFile = GetPasswordFile();
            var encryptedToken = passwordFile.EncryptedToken;
            var encryptedTokenBytes = Convert.FromBase64String(encryptedToken);
            return aes.DecryptEcb(encryptedTokenBytes, PaddingMode.PKCS7).Take(32).ToArray();
        }

        public override bool UserRegistered()
        {
            try
            {
                return _storageEngine.Read(DirectoriesStructure.GetPasswordFilePath()) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a random Guid to be used as a salt
        /// </summary>
        private static string GenerateSalt()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Generates a random Guid to be used as an ecryption token
        /// </summary>
        private static string GenerateToken()
        {
            var token = new byte[32];
            var random = new Random();
            random.NextBytes(token);
            return Convert.ToBase64String(token);
        }

        /// <summary>
        /// Hashes a password with the given salt using SHA256
        /// </summary>
        private static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            return Convert.ToBase64String(hashedPasswordBytes);
        }

        private string EncryptToken(string password, string token)
        {
            using var aes = Aes.Create();
            aes.Key = GetAESKeyFromPassword(password);
            var tokenBytes = Convert.FromBase64String(token);
            var encryptedTokenBytes = aes.EncryptEcb(tokenBytes, PaddingMode.PKCS7);
            return Convert.ToBase64String(encryptedTokenBytes);
        }

        private byte[] GetAESKeyFromPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password))
                .Take(32)
                .ToArray();
        }

        private PasswordFile GetPasswordFile()
        {
            var passwordFileBytes = _storageEngine.Read(DirectoriesStructure.GetPasswordFilePath());
            var passwordFile = JsonSerializer.Deserialize<PasswordFile>(passwordFileBytes)!;
            return passwordFile;
        }

        private void StorePasswordFile(PasswordFile passwordFile)
        {
            var passwordFilePath = DirectoriesStructure.GetPasswordFilePath();
            if (!UserRegistered())
                _storageEngine.Store(passwordFilePath, JsonSerializer.SerializeToUtf8Bytes(passwordFile));
            else
                _storageEngine.Replace(passwordFilePath, JsonSerializer.SerializeToUtf8Bytes(passwordFile));
        }
    }
}