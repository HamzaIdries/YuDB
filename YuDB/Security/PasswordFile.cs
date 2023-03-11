namespace YuDB.Security
{
    /// <summary>
    /// The password file where users credentials are stored. Note that the password is not stored, but rather the
    /// hash of the salt and the password is stored, hence the need to store the salt as well
    /// </summary>
    internal class PasswordFile
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
}