using Microsoft.ClearScript.V8;
using YuDB.Managers;
using YuDB.Security;

namespace YuDB.JavascriptMapping
{
    public class JavascriptUtility
    {
        /// <summary>
        /// Converts an untyped Javascript predicate function into a typed C# function
        /// </summary>
        public static Func<string, bool> ConvertPredicate(dynamic predicate)
        {
            return (string str) => predicate(str);
        }

        /// <summary>
        /// Converts an untyped Javascript updater function into a typed C# function
        /// </summary>
        public static Func<string, string> ConvertUpdater(dynamic updater)
        {
            return (string str) => updater(str);
        }
    }

    /// <summary>
    /// Manages the Javascript interface to the application
    /// </summary>
    public class JavascriptMapping : IDisposable
    {
        private readonly V8ScriptEngine engine;

        private readonly AbstractSecurityEngine securityEngine;

        public JavascriptMapping(AbstractDatabasesManager databasesManager, AbstractSecurityEngine securityEngine)
        {
            engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);

            this.securityEngine = securityEngine;

            // Expose the manager to the script
            engine.AddHostObject("manager", databasesManager);

            // Expose the utility function
            engine.AddHostType("utils", typeof(JavascriptUtility));

#if DEBUG
            engine.AddHostType("console", typeof(Console));
#endif

            LoadScripts();
        }

        public void Dispose()
        {
            engine.Dispose();
        }

        public string Evaluate(string command)
        {
            dynamic result = engine.Evaluate(command);
            return engine.Script.prettyPrint(result);
        }

        /// <summary>
        /// Loads all the extra scripts and functions
        /// </summary>
        private void LoadScripts()
        {
            // Add the clear function
            var clear = Console.Clear;
            engine.Script.clear = clear;

            // Add the exit function
            var exit = () => Environment.Exit(0);
            engine.Script.exit = exit;

            // Add a function to load a JSON document
            var loadDocument = (string path) => engine.Script.JSON.parse(File.ReadAllText(path));
            engine.Script.load = loadDocument;

            // Load the built-in functions
            engine.Execute(File.ReadAllText("./yu.js"));
        }
    }
}