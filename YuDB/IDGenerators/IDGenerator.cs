using System.Text.Json;
using System.Text.Json.Nodes;

namespace YuDB.IDGenerators
{
    /// <summary>
    /// Generates unique ids for each document
    /// </summary>
    public class IDGenerator : AbstractIDGenerator
    {
        /// <summary>
        /// Generates a unique id
        /// </summary>
        public override string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Adds an id field with the name "$id" to the document
        /// </summary>
        public override string AddID(string document, string ID)
        {
            var json = JsonSerializer.Deserialize<JsonObject>(document)!;
            json["$id"] = ID;
            return json.ToString();
        }
    }
}