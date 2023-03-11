namespace YuDB.Security
{
    public class Credentials
    {
        private string username;

        private string password;
        public string Username => username;
        public string Password => password;

        public Credentials(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}