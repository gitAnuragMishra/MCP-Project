using System.Text.Json;
using AIChatService;
using CommonFunctions;
using MCPSharp;
using ViewModels;
using static CommonFunctions.EnumUtility;

namespace MCP_Client.Mcp_Helper
{
    public class ConnectServer
    {
        private readonly MCPClient _client;
        private readonly GeminiChatService _chatService;
        public ConnectServer(MCPClient client, GeminiChatService geminiChatService)
        {
            _client = client;
            _chatService = geminiChatService;
        }

        public async Task<McpRootModel> PrepareRequestBody(string userPrompt)
        {
            var functions = await _client.GetFunctionsAsync();
            var tools = await _client.GetToolsAsync();

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
            var requestBody = await PrepareRequestBody(userPrompt);

            var param = requestBody.Mcp.Function.Parameters.ToDictionary(p => p.Name, p => p.Value);

            var result = await _client.CallToolAsync(
                                    name: requestBody.Mcp.Function.Name,
                                    parameters: requestBody.Mcp.Function.Parameters.ToDictionary(p => p.Name, p => p.Value));

            var serverResponse = result != null ? result.Content[0].Text : "MCP server didn't respond";

            string responseBody = string.Empty;
            /// Need to decide if we want to use the LLM to format the response or just return raw
            responseBody = await PrepareResponseBody(userPrompt, serverResponse);

            return responseBody == string.Empty ? serverResponse : responseBody;
        }
    } 
}
