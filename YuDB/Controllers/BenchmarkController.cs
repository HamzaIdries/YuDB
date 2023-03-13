using Autofac;
using Autofac.Features.AttributeFilters;
using Spectre.Console;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using YuDB.IDGenerators;
using YuDB.Managers;
using YuDB.Storage;
using YuDB.Storage.Filters;

namespace YuDB
{
    /// <summary>
    /// This controller is used to run a benchmark
    /// </summary>
    internal class BenchmarkController
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

        private static int NUMBER_OF_DOCUMENTS = 500;

        private FileFiltersCollection _fileFilters = new FileFiltersCollection();
        private FileIntegrityFilter _integrityFilter = new FileIntegrityFilter(INTEGRITY_KEY);
        private bool _usingCache;

        public BenchmarkController(bool useCached = true)
        {
            _usingCache = useCached;

            // Add the integrity filter to the filters collection
            _fileFilters.Add(_integrityFilter);

            // Register all the components
            var builder = new ContainerBuilder();

            // Register the databases manager
            if (useCached)
                builder.RegisterType<FilteredCachedStorageEngine>()
                    .Keyed<AbstractStorageEngine>("documentsStorageEngine")
                    .WithParameter("filter", _fileFilters)
                    .SingleInstance();
            else
                builder.RegisterType<FilteredStorageEngine>()
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
                var usingCache = _usingCache ? "Using cache" : "Not using cache";
                Write($"Starting benchmark ({usingCache})\n");

                // Delete the root directory if it already exists
                if (Directory.Exists(Config.DatabasesDirectory))
                    Directory.Delete(Config.DatabasesDirectory, true);

                // Create the empty root directory
                Directory.CreateDirectory(Config.DatabasesDirectory);

                // Get the main components
                var manager = scope.Resolve<AbstractDatabasesManager>();

                // Create the test database and collection
                manager.CreateDatabase("db");
                manager.CreateCollection("db", "co", File.ReadAllText("./schema.json"));

                // Insert documents
                Write($"Writing {NUMBER_OF_DOCUMENTS} documents...\n");
                for (int i = 0; i < NUMBER_OF_DOCUMENTS; i++)
                    manager.WriteDocument("db", "co", $"{{\"name\": \"Test\", \"age\": {i}}}");

                Write("Finished writing documents\nReading documents...\n");

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                // Perform random reads based on age
                for (int i = 0; i < NUMBER_OF_DOCUMENTS; i++)
                {
                    var random = new Random();
                    var target = random.Next(NUMBER_OF_DOCUMENTS);
                    manager.ReadDocuments("db", "co", PredicateGenerator(target));
                }

                stopwatch.Stop();

                Write($"Finished reading documents\nTime: {stopwatch.ElapsedMilliseconds}");
            }
        }

        private static void Write(string str)
        {
            AnsiConsole.Write(new Markup($"[yellow bold italic]{str}[/]"));
        }
        private Func<string, bool> PredicateGenerator(int target)
        {
            return (string document) =>
            {
                var obj = JsonSerializer.Deserialize<JsonObject>(document)!;
                return obj["age"]!.AsValue().GetValue<int>() == target;
            };
        }
    }
}