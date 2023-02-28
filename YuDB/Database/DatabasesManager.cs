using System.Collections;
using System.Text;
using YuDB.Constraints;
using YuDB.Security;
using YuDB.Storage;

namespace YuDB.Database
{
    public class DatabasesManager : IDatabasesManager
    {
        private readonly IStorageEngine storageEngine;
        public DatabasesManager(IStorageEngine storageEngine)
        {
            this.storageEngine = storageEngine;
        }
        private ObjectTypeConstraint GetSchema(string databaseName, string collectionName)
        {
            var schemaPath = DirectoriesStructure.GetSchema(databaseName, collectionName);
            var schmeaBytes = storageEngine.ReadDocument(schemaPath);
            var schemaString = Encoding.UTF8.GetString(schmeaBytes);
            return ObjectTypeConstraint.FromString(schemaString);
        }
        public void WriteDocument(string databaseName, string collectionName, string document)
        {
            var schema = GetSchema(databaseName, collectionName);
            if (schema.Validate(document))
            {
                var ID = IDGenerator.GenerateID();
                document = IDGenerator.AddID(document, ID);
                var documentBytes = Encoding.UTF8.GetBytes(document);
                var documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, ID);
                storageEngine.Store(documentPath, documentBytes);
            }
            else
            {
                // TODO: Error handling
                throw new NotImplementedException("Error @ WriteDocument");
            }
        }

        public void ReplaceDocument(string databaseName, string collectionName, string documentID, string updatedDocument)
        {
            var schema = GetSchema(databaseName, collectionName);
            if (schema.Validate(updatedDocument))
            {
                updatedDocument = IDGenerator.AddID(updatedDocument, documentID);
                var documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, documentID);
                var updatedDocumentBytes = Encoding.UTF8.GetBytes(updatedDocument);
                storageEngine.Replace(documentPath, updatedDocumentBytes);
            }
            else
            {
                // TODO: Error handling
                throw new NotImplementedException("Error @ ReplaceDocument");
            }
        }

        public void DeleteDocument(string databaseName, string collectionName, string documentID)
        {
            string documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, documentID);
            storageEngine.Delete(documentPath); // TODO: Error handling
        }

        public string ReadDocument(string databaseName, string collectionName, string documentID)
        {
            string documentPath = DirectoriesStructure.GetDocumentPath(databaseName, collectionName, documentID);
            var documentBytes = storageEngine.ReadDocument(documentPath);
            return Encoding.UTF8.GetString(documentBytes); // TODO: Error handling
        }

        public void CreateDatabase(string databaseName)
        {
            var databasePath = DirectoriesStructure.GetDatabasePath(databaseName);
            Directory.CreateDirectory(databasePath);
        }

        public void CreateCollection(string databaseName, string collectionName, string schema)
        {
            var collectionPath = DirectoriesStructure.GetCollectionPath(databaseName, collectionName);
            var documentsPath = Path.Join(collectionPath, "documents");
            var schemaPath = Path.Join(collectionPath, "schema.json");
            Directory.CreateDirectory(documentsPath);
            // TODO: Error handling: What if the schema is invalid?
            var schemaDocumentBytes = Encoding.UTF8.GetBytes(schema);
            storageEngine.Store(schemaPath, schemaDocumentBytes);
        }
    }
}
