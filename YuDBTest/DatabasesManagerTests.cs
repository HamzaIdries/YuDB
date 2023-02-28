using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using YuDB;
using YuDB.Database;
using YuDB.Storage;

namespace YuDBTest
{
    internal class DatabasesManagerTests
    {
        private string schema;
        private string documentA;
        private string documentB;
        private readonly DatabasesManager databasesManager = new DatabasesManager(StorageEngine.Create());
        [OneTimeSetUp]
        public void SetUp()
        {
            schema = File.ReadAllText("./schema.json");
            documentA = File.ReadAllText("./documentA.json");
            documentB = File.ReadAllText("./documentB.json");
            databasesManager.CreateDatabase("db");
            databasesManager.CreateCollection("db", "co", schema);
        }
        [TearDown]
        public void TearDown()
        {
            Directory.Delete(Config.RootPath, true);
        }
        [Test]
        public void TestWriteDocument()
        {
            databasesManager.WriteDocument("db", "co", documentA);
            var documentsDirectory = Path.Join(Config.RootPath, "db", "co", "documents");
            var storedDocument = File.ReadAllText(Directory.EnumerateFiles(documentsDirectory).Single());
        }
    }
}
