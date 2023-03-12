namespace YuDB.Security
{
    public class Credentials
    {
        private string _password;
        private string _username;
        public Credentials(string username, string password)
        {
            this._username = username;
            this._password = password;
        }

        public string Password => _password;
        public string Username => _username;
    }
}