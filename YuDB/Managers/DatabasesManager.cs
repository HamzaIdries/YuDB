using Autofac.Features.AttributeFilters;
using System.Text;
using YuDB.Constraints;
using YuDB.IDGenerators;
using YuDB.Storage;
using YuDB.Storage.Filters;

namespace YuDB.Managers
{
    public class DatabasesManager : AbstractDatabasesManager
    {
        private readonly AbstractIDGenerator _idGenerator;
        private readonly AbstractStorageEngine _storageEngine;

        public DatabasesManager([KeyFilter("documentsStorageEngine")] AbstractStorageEngine storageEngine, AbstractIDGenerator idGenerator)
        {
            _storageEngine = storageEngine;
            _idGenerator = idGenerator;
        }

        public override void CreateCollection(string databaseName, string collectionName, string schema)
        {
            // Throw an exception if the collection already exists
            var collectionPath = DirectoriesStructure.GetCollectionPath(databaseName, collectionName);
            if (Directory.Exists(collectionPath))
                throw new DatabaseException($"The collection '{databaseName}/{collectionName}' already exists");

            // Otherwise create the directories
            var documentsPath = DirectoriesStructure.GetDocumentsPath(databaseName, collectionName);
            Directory.CreateDirectory(documentsPath);

            // This function is called just so that it throws an exception if the schema is invalid
            ObjectTypeConstraint.FromString(schema);

            // And finally, store the schema
            var schemaDocumentBytes = Encoding.UTF8.GetBytes(schema);
            var schemaPath = DirectoriesStructure.GetSchemaPath(databaseName, collectionName);
            _storageEngine.Store(schemaPath, schemaDocumentBytes);
        }

        public override void CreateDatabase(string databaseName)
        {
            // Attempt to create a new database, but throw an exception if it already exists
            var databasePath = DirectoriesStructure.GetDatabasePath(databaseName);
            if (Directory.Exists(databasePath))
                throw new DatabaseException($"The database {databaseName} already exists");
            Directory.CreateDirectory(databasePath);
        }

        public override void DeleteCollection(string databaseName, string collectionName)
        {
            // Throw an exception if the collection doesn't exist
            var collectionPath = DirectoriesStructure.GetCollectionPath(databaseName, collectionName);
            if (!Directory.Exists(collectionPath))
                throw new DatabaseException($"The collection {databaseName}/{collectionName} does not exists");
            Directory.Delete(collectionPath, true);
        }

        public override void DeleteDatabase(string databaseName)
        {
            // Throw an exception if the database doesn't exist
            var databasePath = DirectoriesStructure.GetDatabasePath(databaseName);
            if (!Directory.Exists(databasePath))
                throw new DatabaseException($"The database {databaseName} does not exists");
            Directory.Delete(databasePath, true);
        }

        public override void DeleteDocument(string databaseName, string collectionName, string documentID)
        {
            string documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, documentID);
            _storageEngine.Delete(documentPath);
        }

        public override int DeleteDocuments(string databaseName, string collectionName, Func<string, bool> predicate)
        {
            // Iterate over each document in the collection
            var documentsDirectory = DirectoriesStructure.GetDocumentsPath(databaseName, collectionName);
            int counter = 0;
            foreach (var documentPath in Directory.EnumerateFiles(documentsDirectory))
            {
                var documentID = Path.GetFileNameWithoutExtension(documentPath);
                var document = ReadDocument(databaseName, collectionName, documentID)!;
                // Return the document if it satisfies the predicate
                if (predicate(document))
                {
                    DeleteDocument(databaseName, collectionName, documentID);
                    counter++;
                }
            }
            return counter;
        }

        public override List<string> ListCollections(string databaseName)
        {
            var databaseDirectory = DirectoriesStructure.GetDatabasePath(databaseName);
            return Directory.EnumerateDirectories(databaseDirectory)
                .Select(Path.GetFileNameWithoutExtension)!
                .ToList()!;
        }

        public override List<string> ListDatabases()
        {
            var databasesDirectory = DirectoriesStructure.GetDatabasesDirectory();
            return Directory.EnumerateDirectories(databasesDirectory)
                .Select(Path.GetFileNameWithoutExtension)!
                .ToList()!;
        }

        public override List<string> ReadDocuments(string databaseName, string collectionName, Func<string, bool> predicate)
        {
            var result = new List<string>();
            // Iterate over each document in the collection
            var documentsDirectory = DirectoriesStructure.GetDocumentsPath(databaseName, collectionName);
            foreach (var documentPath in Directory.EnumerateFiles(documentsDirectory))
            {
                var documentID = Path.GetFileNameWithoutExtension(documentPath);
                try
                {
                    var document = ReadDocument(databaseName, collectionName, documentID);
                    // Return the document if it satisfies the predicate
                    if (predicate(document))
                        result.Add(document);
                }
                catch (IntegrityFailException ex)
                {
                    var policy = Config.UnauthorisedModificationsPolicy;
                    if (policy == "warning")
                        result.Add($"{{\"warning\": \"'{documentID}' was illegaly modified\"}}");
                    else if (policy == "error")
                        throw ex;
                }
            }
            return result.ToList();
        }

        public override int ReplaceDocuments(string databaseName, string collectionName, Func<string, bool> predicate, Func<string, string> updater)
        {
            // Iterate over each document in the collection
            var documentsDirectory = DirectoriesStructure.GetDocumentsPath(databaseName, collectionName);
            int counter = 0;
            foreach (var documentPath in Directory.EnumerateFiles(documentsDirectory))
            {
                var documentID = Path.GetFileNameWithoutExtension(documentPath);
                var document = ReadDocument(databaseName, collectionName, documentID)!;
                // Return the document if it satisfies the predicate
                if (predicate(document))
                {
                    ReplaceDocument(databaseName, collectionName, documentID, updater(document));
                    counter++;
                }
            }
            return counter;
        }

        public override void WriteDocument(string databaseName, string collectionName, string document)
        {
            // First, read the schema and verify the document against the constraints
            var schema = GetCollectionSchema(databaseName, collectionName);
            schema.Validate(document);

            // Then, append an id to the document
            var ID = _idGenerator.GenerateID();
            document = _idGenerator.AddID(document, ID);

            // Finally, store the document
            var documentBytes = Encoding.UTF8.GetBytes(document);
            var documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, ID);
            _storageEngine.Store(documentPath, documentBytes);
        }

        /// <summary>
        /// Reads the schema of the specified collection
        /// </summary>
        private ObjectTypeConstraint GetCollectionSchema(string databaseName, string collectionName)
        {
            try
            {
                var schemaPath = DirectoriesStructure.GetSchemaPath(databaseName, collectionName);
                var schmeaBytes = _storageEngine.Read(schemaPath);
                var schemaString = Encoding.UTF8.GetString(schmeaBytes);
                var schema = ObjectTypeConstraint.FromString(schemaString);
                return schema;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Couldn't read schema of '{databaseName}/{collectionName}'.\nError: {ex.Message}");
            }
        }

        private string ReadDocument(string databaseName, string collectionName, string documentID)
        {
            string documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, documentID);
            var documentBytes = _storageEngine.Read(documentPath);
            return Encoding.UTF8.GetString(documentBytes);
        }

        private void ReplaceDocument(string databaseName, string collectionName, string documentID, string updatedDocument)
        {
            // First, read the schema and verify the updated document against the constraints
            var schema = GetCollectionSchema(databaseName, collectionName);
            schema.Validate(updatedDocument);

            // Then, append the same original id to the updated document
            updatedDocument = _idGenerator.AddID(updatedDocument, documentID);

            // Finally, update the document
            var documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, documentID);
            var updatedDocumentBytes = Encoding.UTF8.GetBytes(updatedDocument);
            _storageEngine.Replace(documentPath, updatedDocumentBytes);
        }
    }
}