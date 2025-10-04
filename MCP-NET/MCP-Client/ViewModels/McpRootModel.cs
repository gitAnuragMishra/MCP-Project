using System.Text.Json.Serialization;

namespace ViewModels
{
    public class McpRootModel
    {
        [JsonPropertyName("mcp")]
        public Mcp Mcp { get; set; }
    }
    public class Mcp
    {
        [JsonPropertyName("function")]
        public McpFunction Function { get; set; }
    }

    public class McpFunction
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parameters")]
        public List<McpParameter> Parameters { get; set; }
    }

    public class McpParameter
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("Value")]
        public object Value { get; set; }
    }
}
