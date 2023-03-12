namespace YuDB.Security
{
    /// <summary>
    /// The password file where users credentials are stored. Note that the password is not stored, but rather the
    /// hash of the salt and the password is stored, hence the need to store the salt as well
    /// </summary>
    internal class PasswordFile
    {
        private readonly string _encryptedToken;
        private readonly string _hashedPassword;
        private readonly string _salt;
        private readonly string _username;

        public PasswordFile(string username, string hashedPassword, string salt, string encryptedToken)
        {
            _username = username;
            _hashedPassword = hashedPassword;
            _salt = salt;
            _encryptedToken = encryptedToken;
        }

        public string EncryptedToken => _encryptedToken;
        public string HashedPassword => _hashedPassword;
        public string Salt => _salt;
        public string Username => _username;
    }
}