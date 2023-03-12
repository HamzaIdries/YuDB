using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace YuDB.Constraints
{
    /// <summary>
    /// Specifies a constraint that a string must specify
    /// </summary>
    [JsonDerivedType(typeof(StringTypeConstraint), "string")]
    [JsonDerivedType(typeof(NumberTypeConstraint), "number")]
    [JsonDerivedType(typeof(BooleanTypeConstraint), "boolean")]
    [JsonDerivedType(typeof(ObjectTypeConstraint), "object")]
    [JsonDerivedType(typeof(ArrayTypeConstraint), "array")]
    public abstract class AbstractConstraint
    {
        /// <summary>
        /// Ensures that a specific node in the JSON tree satisfies the required constraint.
        /// The function throws an exception with the error detail on failure.
        /// </summary>
        /// <param name="document">The string to verify the constraint against</param>
        /// <param name="context">The context array needed to reach the node</param>
        /// <exception cref="DatabaseException"></exception>
        public abstract void Validate(JsonNode document, IEnumerable<string> context);

        /// <summary>
        /// Formats a context array into a string of the form "$.field0.field1..."
        /// </summary>
        protected static string FormatContext(IEnumerable<string> context)
        {
            return string.Join(".", context);
        }

        /// <summary>
        /// Traverses a JSON tree according to the context array to reach a specific node.
        /// </summary>
        /// <param name="document">The JSON tree to be traversed</param>
        /// <param name="context"> The path to reach a specific node in the JSON tree, elements can be
        /// the name of a JSON object field, or in the form "[integer]" to
        /// indicate a specific element in an array.</param>
        /// <returns>The node in the JSON tree specified by the context</returns>
        protected static JsonNode? TraverseContext(JsonNode document, IEnumerable<string> context)
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
                    document = document[field]!;
                else
                    return null;
            }
            return document;
        }
    }
}