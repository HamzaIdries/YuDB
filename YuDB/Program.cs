using System.Security.Cryptography;
using YuDB;
using YuDB.IDGenerators;
using YuDB.JavascriptMapping;
using YuDB.Managers;
using YuDB.Security;
using YuDB.Storage;
using YuDB.Storage.Filters;
using YuDB.Views;

internal class Program
{
    private static byte[] INTEGRITY_KEY = new byte[]
    {
        0x60, 0x1c, 0x86, 0x57,
        0xe5, 0xbe, 0xba, 0xf1,
        0xb4, 0xe2, 0xf6, 0x52,
        0x10, 0x8c, 0x4c, 0x76
    };

    private static byte[] IV = new byte[]
    {
        0x60, 0x1c, 0x86, 0x57,
        0xe5, 0xbe, 0xba, 0xf1,
        0xb4, 0xe2, 0xf6, 0x52,
        0x10, 0x8c, 0x4c, 0x76
    };

    private static void Main(string[] args)
    {
        // Create the root directory
        Directory.CreateDirectory(Config.Get().DatabasesDirectory);

        // Creating the filters
        var integrityFilter = new IntegrityFilter(INTEGRITY_KEY);
        var encryptionFilter = new EncryptionFilter(IV);

        // Initializing the security engine
        var passwordFileStorageEngine = new FilteredCachedStorageEngine(integrityFilter);
        var secrityEngine = new SecurityEngine(passwordFileStorageEngine);

        // Creating the view
        var view = new View();

        // Display the welcome screen
        view.Welcome();

        // Authenticate the user
        if (secrityEngine.IsUserRegistered())
        {
            // Sign in if the user is already registered
            while (true)
            {
                var credentials = view.SignIn();
                if (secrityEngine.Authenticate(credentials))
                {
                    encryptionFilter.SetKey(credentials.Password);
                    break;
                }
            }
        }
        else
        {
            // Sign up if the user is not registered
            var credentials = view.SignUp();
            secrityEngine.CreateUser(credentials);
            encryptionFilter.SetKey(credentials.Password);
        }

        // Create the documents storage engine
        var filters = new FileFiltersCollection(encryptionFilter, integrityFilter);
        var documentsStorageEngine = new FilteredCachedStorageEngine(filters);

        // Create the databases manager
        AbstractDatabasesManager databasesManager = new DatabasesManager(documentsStorageEngine, new IDGenerator());

        // Create the javascript mapping
        using var javascriptMapping = new JavascriptMapping(databasesManager, secrityEngine);

        // Start the main loop
        while (true)
        {
            var prompt = view.GetUserPrompt();
            try
            {
                var result = javascriptMapping.Evaluate(prompt);
                view.DisplayOutput(result, ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                view.DisplayOutput(ex.Message, ConsoleColor.Red);
            }
        }
    }
}