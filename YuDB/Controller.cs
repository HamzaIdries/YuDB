using Autofac;
using YuDB.IDGenerators;
using YuDB.Javascript;
using YuDB.Managers;
using YuDB.Security;
using YuDB.Storage;
using YuDB.Storage.Filters;
using YuDB.Views;

namespace YuDB
{
    internal class Controller
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

        private FileIntegrityFilter integrityFilter = new FileIntegrityFilter(INTEGRITY_KEY);

        private FileFiltersCollection fileFilters = new FileFiltersCollection();

        private IContainer container { set; get; }

        public Controller()
        {
            // Add the integrity filter to the filters collection
            fileFilters.Add(integrityFilter);

            // Register all the components
            var builder = new ContainerBuilder();
            builder.RegisterType<View>().As<AbstractView>().SingleInstance();
            builder.RegisterType<SecurityEngine>().As<AbstractSecurityEngine>().SingleInstance();
            builder.RegisterType<JavascriptMapping>().As<AbstractJavascriptMapping>().SingleInstance();
            builder.RegisterType<DatabasesManager>().As<AbstractDatabasesManager>().SingleInstance();
            builder.RegisterType<IDGenerator>().As<AbstractIDGenerator>().SingleInstance();
            builder.RegisterType<FilteredCachedStorageEngine>().As<AbstractStorageEngine>().SingleInstance();
            builder.RegisterInstance<AbstractFileFilter>(fileFilters);
            container = builder.Build();
        }

        /// <summary>
        /// Starts the application
        /// </summary>
        public void Start()
        {
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the root directory
                Directory.CreateDirectory(Config.Get().DatabasesDirectory);

                // Get the main components
                var view = scope.Resolve<AbstractView>();
                var securityEngine = scope.Resolve<AbstractSecurityEngine>();
                var javascriptMapping = scope.Resolve<AbstractJavascriptMapping>();

                // Display the welcome screen
                view.Welcome();

                // Authenticate the user
                if (securityEngine.UserRegistered())
                {
                    // Sign in if the user is already registered
                    while (true)
                    {
                        var credentials = view.SignIn();
                        if (securityEngine.Authenticate(credentials))
                        {
                            // Add the encryption filter
                            var encryptionFilter = new FileEncryptionFilter(credentials.Password, IV);
                            fileFilters.Add(encryptionFilter);
                            break;
                        }
                    }
                }
                else
                {
                    // Sign up if the user is not registered
                    var credentials = view.SignUp();
                    securityEngine.CreateUser(credentials);

                    // Add the encryption filter
                    var encryptionFilter = new FileEncryptionFilter(credentials.Password, IV);
                    fileFilters.Add(encryptionFilter);
                }

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
    }
}