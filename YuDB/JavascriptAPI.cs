using Microsoft.ClearScript.V8;
using System.Text.Json;
using System.Text.Json.Nodes;
using YuDB.Database;

namespace YuDB
{
    public class Collection
    {
        private readonly string databaseName;
        private readonly string collectionName;
        private readonly IDatabasesManager databasesManager;
        private readonly V8ScriptEngine engine;
        public Collection(
            string databaseName,
            string collectionName,
            string schema,
            IDatabasesManager databasesManager,
            V8ScriptEngine engine)
        {
            this.databaseName = databaseName;
            this.collectionName = collectionName;
            this.databasesManager = databasesManager;
            this.engine = engine;
            var collectionPath = DirectoriesStructure.GetCollectionPath(databaseName, collectionName);
            if (!Directory.Exists(collectionPath))
            {
                databasesManager.CreateCollection(databaseName, collectionName, schema);
            }
        }
        public Collection(
            string databaseName,
            string collectionName,
            IDatabasesManager databasesManager,
            V8ScriptEngine engine)
        {
            this.databaseName = databaseName;
            this.collectionName = collectionName;
            this.databasesManager = databasesManager;
            this.engine = engine;
            var collectionPath = DirectoriesStructure.GetCollectionPath(databaseName, collectionName);
            if (!Directory.Exists(collectionPath))
            {
                // TODO: Error handling
                throw new NotImplementedException();
            }
        }
        public dynamic add(params dynamic[] documents)
        {
            documents
                .Select(document => engine.Script.JSON.stringify(document))
                .ToList()
                .ForEach(document => databasesManager.WriteDocument(databaseName, collectionName, document));
            return documents.Length;
        }
        private dynamic readDocument(string documentID)
        {
            var document = databasesManager.ReadDocument(databaseName, collectionName, documentID);
            return engine.Evaluate($"JSON.parse(`{document}`)");
        }
        public dynamic read(dynamic predicate)
        {
            var func = readDocument;
            return select(predicate).map(func);
        }
        public dynamic select(dynamic predicate)
        {
            var results = new JsonArray();
            var documentsPath = DirectoriesStructure.GetDocumentsPath(databaseName, collectionName);
            foreach (var documentPath in Directory.EnumerateFiles(documentsPath))
            {
                var documentID = Path.GetFileNameWithoutExtension(documentPath);
                var fileContent = databasesManager.ReadDocument(databaseName, collectionName, documentID);
                var json = engine.Evaluate($"JSON.parse(`{fileContent}`)");
                if (predicate(json))
                {
                    results.Add(Path.GetFileNameWithoutExtension(documentPath));
                }
            }
            return engine.Evaluate($"JSON.parse(`{results}`)");
        }
        public dynamic delete(dynamic predicate)
        {
            var documents = select(predicate);
            var func = (string documentID) => databasesManager.DeleteDocument(databaseName, collectionName, documentID);
            documents.forEach(func);
            return documents.length;
        }
        public dynamic update(dynamic predicate, dynamic updater)
        {
            var documents = select(predicate);
            foreach (dynamic documentID in documents)
            {
                dynamic oldDocument = readDocument(documentID);
                dynamic updatedDocument = updater(oldDocument);
                string updatedDocumentString = engine.Script.JSON.stringify(updatedDocument);
                databasesManager.ReplaceDocument(databaseName, collectionName, documentID, updatedDocumentString);
            }
            return documents.length;
        }
    }
    public class JavascriptAPI : IDisposable
    {
        private readonly IDatabasesManager databasesManager;
        private readonly V8ScriptEngine engine;
        private void AddCollection(string databaseName, string collectionName)
        {
            engine.Script[Config.Get().RootVariableName][databaseName][collectionName] = new Collection(
                databaseName,
                collectionName,
                databasesManager,
                engine
            );
        }
        private void AddDatabase(string databaseName)
        {
            engine.Execute($"{Config.Get().RootVariableName}.{databaseName} = {{}}");

            // Add already existing collections
            var collections = Directory
                .EnumerateDirectories(DirectoriesStructure.GetDatabasePath(databaseName))
                .Select(Path.GetFileName);
            foreach (var collection in collections)
                AddCollection(databaseName, collection);

            // Add the `createCollection` method to the database
            var createCollection = (string collectionName, dynamic schema) =>
            {
                var schemaString = engine.Script.JSON.stringify(schema) as string;
                databasesManager.CreateCollection(databaseName, collectionName, schemaString);
                AddCollection(databaseName, collectionName);
            };
            engine.Script[Config.Get().RootVariableName][databaseName]["createCollection"] = createCollection;
        }
        private void LoadAdditionalScripts()
        {
            // The pretty print function converts a JSON into a formatted string with
            // indentation
            engine.Execute(@"
                function prettyPrint(obj) {
                    return JSON.stringify(obj, null, 2);
                }");
            // Load the built-in functions
            engine.Execute(File.ReadAllText("./yu.js"));
            var clear = Console.Clear;
            engine.Script.clear = clear;
            var exit = () => Environment.Exit(0);
            engine.Script.exit = exit;
            // A function to load a JSON document
            var loadDocument = (string path) => engine.Script.JSON.parse(File.ReadAllText(path));
            engine.Script.loadDocument = loadDocument;
        }
        public JavascriptAPI(IDatabasesManager databasesManager)
        {
            this.databasesManager = databasesManager;
            engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);

            LoadAdditionalScripts();

            // Creates the root variable
            engine.Execute($"{Config.Get().RootVariableName} = {{}}");

            // Add already existing databases
            var databases = Directory
                .EnumerateDirectories(DirectoriesStructure.GetDatabasesDirectory())
                .Select(dir => Path.GetFileName(dir)!);
            foreach (var database in databases)
                AddDatabase(database);

            // Add the `createDatabase` function to the script
            var createDatabase = (string databaseName) =>
            {
                databasesManager.CreateDatabase(databaseName);
                AddDatabase(databaseName);
            };
            engine.Script.createDatabase = createDatabase;
        }
        public void Dispose()
        {
            engine.Dispose();
        }
        public string Evaluate(string command)
        {
            try
            {
                dynamic result = engine.Evaluate(command);
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    return engine.Script.prettyPrint(result);
                } catch (Exception)
                {
                    return "";
                }
            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                return ex.Message;
            }
        }
    }
}
