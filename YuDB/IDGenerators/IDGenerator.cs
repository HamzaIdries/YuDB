using System.Text.Json;
using System.Text.Json.Nodes;

namespace YuDB.IDGenerators
{
    public class IDGenerator : AbstractIDGenerator
    {
        public override string AddID(string document, string ID)
        {
            var json = JsonSerializer.Deserialize<JsonObject>(document)!;
            json["$id"] = ID;
            return json.ToString();
        }

        public override string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}