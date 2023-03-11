using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using YuDB.Storage;

namespace YuDB.Security
{
    public class SecurityEngine : AbstractSecurityEngine
    {
        private readonly AbstractStorageEngine storageEngine;

        public SecurityEngine(AbstractStorageEngine storageEngine)
        {
            this.storageEngine = storageEngine;
        }

        /// <summary>
        /// Generates a random Guid to be used as a salt. This is valid since Guid is sufficently
        /// long and uses a fairly secure random number generation algorithm
        /// </summary>
        private static string GenerateSalt()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Hashes a password with the given salt using SHA256
        /// </summary>
        private static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                var sb = new StringBuilder();
                foreach (var b in hashedPasswordBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private PasswordFile GetPasswordFile()
        {
            var passwordFileBytes = storageEngine.Read(DirectoriesStructure.GetPasswordFilePath());
            var passwordFile = JsonSerializer.Deserialize<PasswordFile>(passwordFileBytes)!;
            return passwordFile;
        }

        public override bool Authenticate(Credentials credentials)
        {
            PasswordFile passwordFile = GetPasswordFile();
            return
                credentials.Username == passwordFile.Username &&
                HashPassword(credentials.Password, passwordFile.Salt) == passwordFile.HashedPassword;
        }

        public override void CreateUser(Credentials credentials)
        {
            var passwordFilePath = DirectoriesStructure.GetPasswordFilePath();
            string salt = GenerateSalt();
            var passwordFile = new PasswordFile(
                credentials.Username,
                HashPassword(credentials.Password, salt),
                salt);
            storageEngine.Store(passwordFilePath, JsonSerializer.SerializeToUtf8Bytes(passwordFile));
        }

        public override bool IsUserRegistered()
        {
            try
            {
                return storageEngine.Read(DirectoriesStructure.GetPasswordFilePath()) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}