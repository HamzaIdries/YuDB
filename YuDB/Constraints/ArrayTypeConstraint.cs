using System.Text.Json.Nodes;

namespace YuDB.Constraints
{
    /// <summary>
    /// Ensure that JSON node is an array where all the elements are of the same type
    /// </summary>
    public class ArrayTypeConstraint : AbstractConstraint
    {
        private readonly AbstractConstraint _element;
        public ArrayTypeConstraint(AbstractConstraint element)
        {
            this._element = element;
        }

        public AbstractConstraint Element => _element;
        public override void Validate(JsonNode document, IEnumerable<string> context)
        {
            var current = TraverseContext(document, context)!;
            try
            {
                var arr = current.AsArray();
                for (int i = 0; i < arr.Count; i++)
                {
                    var newContext = new List<string>(context) { string.Format("[{0}]", i) };
                    _element.Validate(document, newContext);
                }
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"The node at '{FormatContext(context)}' is not a valid JSON array");
            }
        }
    }
}