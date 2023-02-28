using YuDB;
using YuDB.Storage;

namespace YuDBTest
{
    internal class StorageEngineTests
    {
        private static readonly string document = "{\"title\": \"Test\"}";
        private static readonly string updatedDocument = "{\"title\": \"Updated\"}";
        private static readonly string documentPath = Path.Join(Config.RootPath, "a.json");
        private static readonly StorageEngine storage = StorageEngine.Create();
        [Test]
        public void TestReplace()
        {
            storage.Store(document, documentPath);
            storage.Replace(updatedDocument, documentPath);
            Assert.That(
                Directory.EnumerateFiles(Config.RootPath).Count,
                Is.EqualTo(1)
            );
            Assert.That(
                storage.GetAllDocuments(Config.RootPath).Single(),
                Is.EqualTo(updatedDocument)
            );
        }
        [Test]
        public void TestDelete()
        {
            storage.Store(document, documentPath);
            storage.Delete(documentPath);
            Assert.IsTrue(
                Directory.EnumerateFiles(Config.RootPath).Count() == 0
            );
        }
        [Test]
        public void TestGetDocument()
        {
            storage.Store(document, documentPath);
            Assert.That(
                storage.GetDocument(documentPath),
                Is.EqualTo(document)
            );
        }
        [Test]
        public void TestGetAllDocuments()
        {
            storage.Store(document, documentPath);
            Assert.That(
                storage.GetAllDocuments(Config.RootPath).Single(),
                Is.EqualTo(document)
            );
        }
        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(Config.RootPath);
        }
        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(Config.RootPath, true);
        }
    }
}