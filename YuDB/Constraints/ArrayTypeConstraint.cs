using System.Text.Json.Nodes;

namespace YuDB.Constraints
{
    public class ArrayTypeConstraint : ITypeConstraint
    {
        private readonly ITypeConstraint element;
        public ITypeConstraint Element => element;

        public ArrayTypeConstraint(ITypeConstraint element)
        {
            this.element = element;
        }
        public bool Validate(JsonNode document, IEnumerable<string> context)
        {
            JsonNode current = ITypeConstraint.TraverseContext(document, context)!;
            if (current != null)
            {
                try
                {
                    var arr = current.AsArray();
                    for (int i = 0; i < arr.Count; i++)
                    {
                        var newContext = new List<string>(context) { string.Format("[{0}]", i) };
                        if (!element.Validate(document, newContext))
                            return false;
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                // TODO: Handle exceptions
                throw new Exception();
            }
        }
    }
}
