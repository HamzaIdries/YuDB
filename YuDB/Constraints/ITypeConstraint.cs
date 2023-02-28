using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace YuDB.Constraints
{
    [JsonDerivedType(typeof(StringTypeConstraint),  "string")]
    [JsonDerivedType(typeof(NumberTypeConstraint),  "number")]
    [JsonDerivedType(typeof(BooleanTypeConstraint), "boolean")]
    [JsonDerivedType(typeof(ObjectTypeConstraint),  "object")]
    [JsonDerivedType(typeof(ArrayTypeConstraint),   "array")]
    public interface ITypeConstraint
    {
        bool Validate(JsonNode document, IEnumerable<string> context);

        static JsonNode? TraverseContext(JsonNode document, IEnumerable<string> context)
        {
            foreach (var field in context)
            {
                var match = Regex.Match(field, @"^ *\[ *(\d+) *\] *$");
                if (match.Success)
                {
                    var index = int.Parse(match.Groups[1].Value);
                    document = document[index]!;
                }
                else if (document[field] != null)
                {
                    document = document[field]!;
                }
                else
                {
                    return null;
                }
            }
            return document;

        }
    }
}
