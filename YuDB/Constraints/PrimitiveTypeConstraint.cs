using System.Text.Json.Nodes;

namespace YuDB.Constraints
{
    abstract class PrimitiveTypeConstraint<T> : ITypeConstraint
    {
        public bool Validate(JsonNode document, IEnumerable<string> context)
        {
            var current = ITypeConstraint.TraverseContext(document, context)!;
            try
            {
                current.GetValue<T>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}