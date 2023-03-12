using System.Text.Json;
using System.Text.Json.Nodes;

namespace YuDB.Constraints
{
    /// <summary>
    /// Ensures that a JSON node is a JSON object
    /// </summary>
    internal class ObjectTypeConstraint : AbstractConstraint
    {
        private Dictionary<string, AbstractConstraint> _properties = new Dictionary<string, AbstractConstraint>();

        private ISet<string> _required = new HashSet<string>();

        public Dictionary<string, AbstractConstraint> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        public ISet<string> Required
        {
            get { return _required; }
            set { _required = value; }
        }

        /// <summary>
        /// Creates an instance of ObjectTypeConstraint from the provided string.
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        public static ObjectTypeConstraint FromString(string schemaString)
        {
            try
            {
                return JsonSerializer.Deserialize<ObjectTypeConstraint>(
                    schemaString,
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    }
                )!;
            }
            catch (Exception)
            {
                throw new DatabaseException("The provided string is not a valid schema");
            }
        }

        public override void Validate(JsonNode document, IEnumerable<string> context)
        {
            var current = TraverseContext(document, context)!;
            try
            {
                var obj = current.AsObject();
                foreach (var property in _properties)
                {
                    var newContext = new List<string>(context) { property.Key };
                    var jsonNode = TraverseContext(document, newContext);
                    if (jsonNode == null && _required.Contains(property.Key))
                        throw new DatabaseException($"The field '{property.Key}' is missing but is required");
                    property.Value.Validate(document, newContext);
                }
                // If strict mode is enabled, ensure that new additional undefined fields are used
                if (Config.StrictMode)
                {
                    foreach (var entry in obj)
                    {
                        if (!_properties.ContainsKey(entry.Key))
                            throw new DatabaseException($"The field {entry.Key} is not defined by the schema, and strict mode is enabled");
                    }
                }
            }
            catch (InvalidOperationException)
            {
                throw new DatabaseException($"The node at '{FormatContext(context)}' is not a valid JSON object");
            }
        }

        /// <summary>
        /// A convenience method that validates strings directly, without the need to specify
        /// the context or convert the string into a JsonNode
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        public void Validate(string document)
        {
            try
            {
                var json = JsonSerializer.Deserialize<JsonNode>(document)!;
                Validate(json, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                if (ex is JsonException)
                    throw new DatabaseException("The provided string is not a valid JSON object");
                else if (ex is DatabaseException)
                    throw new DatabaseException(ex.Message);
            }
        }
    }
}