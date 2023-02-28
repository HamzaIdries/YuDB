using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using YuDB.Storage;

namespace YuDB.Security
{
    class PasswordFile
    {
        private readonly string username;
        private readonly string hashedPassword;
        private readonly string salt;

        public string Username => username;
        public string HashedPassword => hashedPassword;
        public string Salt => salt;

        public PasswordFile(string username, string hashedPassword, string salt)
        {
            this.username = username;
            this.hashedPassword = hashedPassword;
            this.salt = salt;
        }
    }
    public class SecurityEngine : ISecurityEngine
    {
        private readonly IStorageEngine storageEngine;
        public SecurityEngine(IStorageEngine storageEngine)
        {
            this.storageEngine = storageEngine;
        }
        private static string GenerateSalt()
        {
            return Guid.NewGuid().ToString();
        }
        private static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                var sb = new StringBuilder();
                foreach (var b in hashedPasswordBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        private void StorePasswordFile(string username, string password)
        {
            if (!File.Exists(Config.Get().PasswordFilePath))
            {
                string salt = GenerateSalt();
                var passwordFile = new PasswordFile(
                    username,
                    HashPassword(password, salt),
                    salt
                );
                storageEngine.Store(Config.Get().PasswordFilePath, JsonSerializer.SerializeToUtf8Bytes(passwordFile));
            }
        }
        public bool Authenticate(string username, string password)
        {
            var passwordFile = JsonSerializer.Deserialize<PasswordFile>(File.ReadAllText(Config.Get().PasswordFilePath))!;
            return username == passwordFile.Username &&
                    HashPassword(password, passwordFile.Salt) == passwordFile.HashedPassword;
        }

        public void ChangePassword(string newPassword)
        {
            var passwordFile = JsonSerializer.Deserialize<PasswordFile>(File.ReadAllText(Config.Get().PasswordFilePath))!;
            var salt = GenerateSalt();
            var newPasswordFile = new PasswordFile(
                passwordFile.Username,
                HashPassword(newPassword, salt),
                salt
            )!;
            storageEngine.Store(Config.Get().PasswordFilePath, JsonSerializer.SerializeToUtf8Bytes(newPasswordFile));
        }

        public void ChangeUsername(string newUsername)
        {
            var passwordFile = JsonSerializer.Deserialize<PasswordFile>(File.ReadAllText(Config.Get().PasswordFilePath))!;
            var newPasswordFile = new PasswordFile(
                newUsername,
                passwordFile.HashedPassword,
                passwordFile.Salt
            )!;
            storageEngine.Store(Config.Get().PasswordFilePath, JsonSerializer.SerializeToUtf8Bytes(passwordFile));
        }
    }
}
