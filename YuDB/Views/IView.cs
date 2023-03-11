using YuDB.Security;

namespace YuDB.Views
{
    /// <summary>
    /// Represents the part of the application that the user interacts with
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Displays output after command evaluation
        /// </summary>
        void DisplayOutput(string output, ConsoleColor color);

        /// <summary>
        /// Gets a command from the users
        /// </summary>
        string GetUserPrompt();

        /// <summary>
        /// Displays a screen that prompts user to sign in
        /// </summary>
        /// <returns>The user's credentials</returns>
        Credentials SignIn();

        /// <summary>
        /// Displays a screen that prompts user to sign up
        /// </summary>
        /// <returns>The user's credentials</returns>
        Credentials SignUp();

        /// <summary>
        /// Displays a welcome screen
        /// </summary>
        void Welcome();
    }
}