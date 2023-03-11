using YuDB.Security;

namespace YuDB.Views
{
    /// <summary>
    /// Represents the part of the application that the user interacts with
    /// </summary>
    public abstract class AbstractView
    {
        /// <summary>
        /// Displays output after command evaluation
        /// </summary>
        public abstract void DisplayOutput(string output, ConsoleColor color);

        /// <summary>
        /// Gets a command from the users
        /// </summary>
        public abstract string GetUserPrompt();

        /// <summary>
        /// Displays a screen that prompts user to sign in
        /// </summary>
        /// <returns>The user's credentials</returns>
        public abstract Credentials SignIn();

        /// <summary>
        /// Displays a screen that prompts user to sign up
        /// </summary>
        /// <returns>The user's credentials</returns>
        public abstract Credentials SignUp();

        /// <summary>
        /// Displays a welcome screen
        /// </summary>
        public abstract void Welcome();
    }
}