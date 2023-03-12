using System.Text.Json.Nodes;

namespace YuDB.Constraints
{
    /// <summary>
    /// Ensures that a JSON node is a valid primitive of type T
    /// </summary>
    internal abstract class PrimitiveTypeConstraint<T> : AbstractConstraint
    {
        public override void Validate(JsonNode document, IEnumerable<string> context)
        {
            var current = TraverseContext(document, context)!;
            try
            {
                current.GetValue<T>();
            }
            catch (Exception)
            {
                throw new DatabaseException($"The field {FormatContext(context)} must be a {typeof(T).Name}");
            }
        }
    }
}