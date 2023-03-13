using Autofac;
using Autofac.Features.AttributeFilters;
using YuDB.Backup;
using YuDB.IDGenerators;
using YuDB.Javascript;
using YuDB.Managers;
using YuDB.Security;
using YuDB.Storage;
using YuDB.Storage.Filters;
using YuDB.Views;

namespace YuDB
{
    /// <summary>
    /// The controller is resonsible for instantiating all components and connecting dependencies
    /// together. It also provides that `Start` method which starts the entire application
    /// </summary>
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

        private static byte[] JAVASCRIPT_STORAGE_ENGINE_KEY = new byte[]
        {
            0xc7, 0xe3, 0x64, 0xc3,
            0xfb, 0xfd, 0x57, 0x32,
            0x5b, 0xcb, 0xdf, 0xde,
            0x12, 0xa9, 0x48, 0x6c
        };

        private FileEncryptionFilter _defaultEncryptionFilter = new FileEncryptionFilter(JAVASCRIPT_STORAGE_ENGINE_KEY, IV);
        private FileFiltersCollection _fileFilters = new FileFiltersCollection();
        private FileIntegrityFilter _integrityFilter = new FileIntegrityFilter(INTEGRITY_KEY);

        public Controller()
        {
            // Add the integrity filter to the filters collection
            _fileFilters.Add(_integrityFilter);

            // Register all the components
            var builder = new ContainerBuilder();

            // Register the view
            builder.RegisterType<View>()
                .As<AbstractView>()
                .SingleInstance();

            // Register the default storage engine that is to be used by the javascript mapping and the security engine
            builder.RegisterType<FilteredCachedStorageEngine>()
                .Keyed<AbstractStorageEngine>("defaultStorageEngine")
                .WithParameter("filter", new FileFiltersCollection(_integrityFilter, _defaultEncryptionFilter))
                .SingleInstance();

            // Register the security engine
            builder.RegisterType<SecurityEngine>()
                .As<AbstractSecurityEngine>()
                .SingleInstance()
                .WithAttributeFiltering();

            // Register the javascript mapping
            builder.RegisterType<JavascriptMapping>()
                .As<AbstractJavascriptMapping>()
                .SingleInstance()
                .WithAttributeFiltering();

            // Register the databases manager
            builder.RegisterType<FilteredCachedStorageEngine>()
                .Keyed<AbstractStorageEngine>("documentsStorageEngine")
                .WithParameter("filter", _fileFilters)
                .SingleInstance();
            builder.RegisterType<IDGenerator>()
                .As<AbstractIDGenerator>()
                .SingleInstance();
            builder.RegisterType<DatabasesManager>()
                .As<AbstractDatabasesManager>()
                .SingleInstance()
                .WithAttributeFiltering();

            // Register the backup manager
            builder.RegisterType<BackupManager>()
                .As<AbstractBackupManager>()
                .SingleInstance();

            container = builder.Build();
        }

        private IContainer container { set; get; }

        /// <summary>
        /// Starts the application
        /// </summary>
        public void Start()
        {
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the root directory
                Directory.CreateDirectory(Config.DatabasesDirectory);

                // Get the main components
                var view = scope.Resolve<AbstractView>();
                var securityEngine = scope.Resolve<AbstractSecurityEngine>();
                var javascriptMapping = scope.Resolve<AbstractJavascriptMapping>();

                // Display the welcome screen
                view.Welcome();

                // Authenticate the user
                var credentials = securityEngine.UserRegistered()
                    ? view.SignIn(securityEngine.Authenticate)
                    : view.SignUp();

                // Store the user's credentials at sign up
                if (!securityEngine.UserRegistered())
                    securityEngine.CreateUser(credentials);

                // Add the encryption filter
                var encryptionFilter = new FileEncryptionFilter(securityEngine.GetToken(credentials.Password), IV);
                _fileFilters.Add(encryptionFilter);

                // Start the main loop
                while (true)
                {
                    var prompt = view.GetUserPrompt();
                    try
                    {
                        var result = javascriptMapping.Evaluate(prompt);
                        if (result != null)
                            view.DisplayOutput(result);
                    }
                    catch (Exception ex)
                    {
                        view.DisplayError(ex.Message);
                    }
                }
            }
        }
    }
}