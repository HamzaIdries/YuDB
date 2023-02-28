using System.Security.Cryptography;
using YuDB;
using YuDB.Database;
using YuDB.Storage;
using YuDB.Storage.Filters;

var INTEGRITY_KEY = new byte[]
{ 
    0x60, 0x1c, 0x86, 0x57,
    0xe5, 0xbe, 0xba, 0xf1,
    0xb4, 0xe2, 0xf6, 0x52,
    0x10, 0x8c, 0x4c, 0x76
};

IStorageEngine storageEngine = StorageEngine.Create();
using var aes = Aes.Create();
IFileFilter filters = new FileFiltersCollection(
    new EncryptionFilter(aes.Key, aes.IV),
    new IntegrityFilter(INTEGRITY_KEY)
);
IStorageEngine filteredStorageEngine = FilteredStorageEngine.Create(filters);
IDatabasesManager databasesManager = new DatabasesManager(filteredStorageEngine);
using var javascriptAPI = new JavascriptAPI(databasesManager);

while (true)
{
    Console.Write("> ");
    var prompt = Console.ReadLine()!;
    Console.WriteLine(javascriptAPI.Evaluate(prompt));
    Console.ResetColor();
}