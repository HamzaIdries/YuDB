using YuDB.Security;

namespace YuDB.Views
{
    /// <summary>
    /// Represents the part of the application that the user interacts with
    /// </summary>
    public abstract class AbstractView
    {
        /// <summary>
        /// Prompts user to change password
        /// </summary>
        public abstract List<string> ChangePassword();

        /// <summary>
        /// Prompts user to change username
        /// </summary>
        public abstract string ChangeUsername();

        /// <summary>
        /// Clears the screen
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Displays an error message
        /// </summary>
        public abstract void DisplayError(string error);

        /// <summary>
        /// Displays output after command evaluation
        /// </summary>
        public abstract void DisplayOutput(string output);

        /// <summary>
        /// Gets a command from the users
        /// </summary>
        public abstract string GetUserPrompt();

        /// <summary>
        /// Displays a screen that prompts user to sign in
        /// </summary>
        /// <returns>The user's credentials</returns>
        public abstract Credentials SignIn(Func<Credentials, bool> authenticate);

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