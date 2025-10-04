using System.Text.Json;

namespace MCPServer1.Helpers
{
    public class JsonHelper
    {
        public static string Serialize<T>(T model)
        {
            var jsonOpts = new JsonSerializerOptions { WriteIndented = true };
            string result = JsonSerializer.Serialize(model, jsonOpts);

            return result;
        }
    }
}
