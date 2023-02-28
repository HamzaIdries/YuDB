using System.Text.Json;
using System.Text.Json.Nodes;

namespace YuDB
{
    public class IDGenerator
    {
        public static string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
        public static string AddID(string document, string ID)
        {
            var json = JsonSerializer.Deserialize<JsonObject>(document);
            json["$id"] = ID;
            return json.ToString();
        }
    }
}
