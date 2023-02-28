using System.Text.Json;
using System.Text.Json.Nodes;

namespace YuDB.Constraints
{
    class ObjectTypeConstraint : ITypeConstraint
    {
        private Dictionary<string, ITypeConstraint> properties = new Dictionary<string, ITypeConstraint>();
        public Dictionary<string, ITypeConstraint> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
        private ISet<string> required = new HashSet<string>();
        public ISet<string> Required
        {
            get { return required; }
            set { required = value; }
        }
        public static ObjectTypeConstraint FromString(string schemaString)
        {
            return JsonSerializer.Deserialize<ObjectTypeConstraint>(
                schemaString,
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }
            )!;
        }
        public bool Validate(JsonNode document, IEnumerable<string> context)
        {
            var current = ITypeConstraint.TraverseContext(document, context)!;
            try
            {
                current.AsObject();
                foreach (var property in properties)
                {
                    List<string> newContext = new List<string>(context) { property.Key };
                    var jsonNode = ITypeConstraint.TraverseContext(document, newContext);
                    if (jsonNode == null && required.Contains(property.Key))
                    {
                        return false;
                    }
                    else if (jsonNode != null && !property.Value.Validate(document, newContext))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Validate(string document)
        {
            var json = JsonSerializer.Deserialize<JsonNode>(document);
            if (json != null)
            {
                return Validate(json, Enumerable.Empty<string>());
            }
            else
            {
                // TODO: Error handling
                throw new NotImplementedException("Error @ Validate");
            }
        }
    }
}
