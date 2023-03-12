namespace YuDB.Javascript
{
    /// <summary>
    /// Manages the Javascript interface to the application
    /// </summary>
    public abstract class AbstractJavascriptMapping
    {
        /// <summary>
        /// Evaluates a Javascript command
        /// </summary>
        public abstract string? Evaluate(string command);
    }
}