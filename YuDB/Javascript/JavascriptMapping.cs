using Autofac.Features.AttributeFilters;
using Microsoft.ClearScript.V8;
using System.Text;
using YuDB.Backup;
using YuDB.Managers;
using YuDB.Security;
using YuDB.Storage;
using YuDB.Views;

namespace YuDB.Javascript
{
    public class JavascriptMapping : AbstractJavascriptMapping, IDisposable
    {
        private readonly V8ScriptEngine _scriptEngine;

        public JavascriptMapping(
            AbstractDatabasesManager databasesManager,
            AbstractSecurityEngine securityEngine,
            [KeyFilter("defaultStorageEngine")] AbstractStorageEngine storageEngine,
            AbstractView view,
            AbstractBackupManager backupManager)
        {
            _scriptEngine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);

            // Expose the manager to the script
            _scriptEngine.AddHostObject("manager", databasesManager);

            // Expose the security engine to the script
            _scriptEngine.AddHostObject("security", securityEngine);

            // Expose the utility class
            _scriptEngine.AddHostType("utils", typeof(JavascriptUtility));

            // Expose the view
            _scriptEngine.AddHostObject("view", view);

            // Expose the backup manager
            _scriptEngine.AddHostObject("backupManager", backupManager);

            // Add the exit function
            var exit = () => Environment.Exit(0);
            _scriptEngine.Script.exit = exit;

            // Add a function to load a JSON document
            var loadDocument = (string path) => _scriptEngine.Script.JSON.parse(File.ReadAllText(path));
            _scriptEngine.Script.load = loadDocument;

            // Load the built-in functions
            _scriptEngine.Execute(Encoding.UTF8.GetString(storageEngine.Read("./yu.js")));
        }

        public void Dispose()
        {
            _scriptEngine.Dispose();
        }

        public override string? Evaluate(string command)
        {
            dynamic result = _scriptEngine.Evaluate(command);
            try
            {
                return _scriptEngine.Script.JSON.stringify(result);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

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
}