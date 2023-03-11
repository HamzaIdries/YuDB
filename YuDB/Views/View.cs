using YuDB.Security;

namespace YuDB.Views
{
    public class View : AbstractView
    {
        private static int MAX_USERNAME_SIZE = 512;

        private static int MAX_PASSWORD_SIZE = 512;

        public override void DisplayOutput(string output, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(output);
            Console.ResetColor();
        }

        public override string GetUserPrompt()
        {
            Console.Write("> ");
            return Console.ReadLine()!;
        }

        private string ReadUsername()
        {
            string? username = null;
            while (username == null)
            {
                Console.Write("Username: ");
                username = Console.ReadLine()!;
                if (username.Length < 1 || username.Length >= MAX_USERNAME_SIZE)
                    username = null;
            }
            return username;
        }

        private string ReadPassword()
        {
            string? password = null;
            while (password == null)
            {
                Console.Write("Password: ");
                password = Console.ReadLine()!;
                if (password.Length < 8 || password.Length >= MAX_PASSWORD_SIZE)
                    password = null;
            }
            return password;
        }

        public override Credentials SignIn()
        {
            Console.WriteLine("Please enter your credentials to sign in.");
            return new Credentials(ReadUsername(), ReadPassword());
        }

        public override Credentials SignUp()
        {
            Console.WriteLine("Please enter your credentials to start.");
            return new Credentials(ReadUsername(), ReadPassword());
        }

        public override void Welcome()
        {
            Console.WriteLine(ViewUtility.ContainWithinRectangle(17, " Welcome to YuDB"));
        }
    }
}