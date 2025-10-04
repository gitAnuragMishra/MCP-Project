using System.Text.Json;
using AIChatService;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ViewModels;
using static CommonFunctions.EnumUtility;


namespace CommonFunctions
{
    public class ConnectServerHttp
    {
        private readonly ICustomMcpClientFactory _clientFactory;
        private readonly GeminiChatServiceWeb _chatService;
        public ConnectServerHttp(ICustomMcpClientFactory client, GeminiChatServiceWeb geminiChatService)
        {
            _clientFactory = client;
            _chatService = geminiChatService;
        }

        public async Task<McpRootModel> PrepareRequestBody(string userPrompt)
        {
            var _client = await _clientFactory.CreateAsync();

            var functions = await _client.ListToolsAsync();
            //var tools = await _client.GetToolsAsync();
            var name = functions[1].Name;
            var schema = functions[1].JsonSchema;

            var catalog = new
            {
                catalog = new
                {
                    functions = functions.Select(function => new
                    {
                        name = function.Name,
                        schema = function.JsonSchema
                    })
                }
            };

            string functionCatalog = JsonHelper.Serialize(catalog);

            string prompt = @$"
                                CATALOG:
                                {functionCatalog}

                                USER REQUEST:
                                {userPrompt}

                                Respond with JSON only. Begin your output with ""{{"" and end with ""}}"".
                                JSON_ONLY_END
                              ";

            var llmResponse = await _chatService.SendMessageAsync(prompt, MessageType.MCPRequest);

            string json = JsonHelper.ExtractFirstJsonObject(llmResponse);
            McpRootModel mcpRoot = JsonSerializer.Deserialize<McpRootModel>(json) ?? new McpRootModel();

            return mcpRoot;
        }

        public async Task<string> PrepareResponseBody(string prompt, string serverResponse)
        {
            var llmResponse = await _chatService.SendMessageAsync(prompt, MessageType.MCPResponse, serverResponse);
            return llmResponse;
        }

        public async Task<string> InvokeMCP(string userPrompt)
        {
            var _client = await _clientFactory.CreateAsync();

            var requestBody = await PrepareRequestBody(userPrompt);
            var param = requestBody.Mcp.Function.Parameters.ToDictionary(p => p.Name, p => p.Value);

            var serverResponse = string.Empty;
            try
            {
                var result = await _client.CallToolAsync(requestBody.Mcp.Function.Name,param);

                if (result != null && result.IsError != true)
                {
                    /// Just extracts the text content, can customize to get images/files etc
                    serverResponse = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "No response from MCP server"; 
                }

            }
            catch (Exception)
            {
                serverResponse = "MCP server didn't respond";
            }

            string responseBody = string.Empty;

            responseBody = await PrepareResponseBody(userPrompt, serverResponse);

            return responseBody == string.Empty ? serverResponse : responseBody;
        }
    }
}
