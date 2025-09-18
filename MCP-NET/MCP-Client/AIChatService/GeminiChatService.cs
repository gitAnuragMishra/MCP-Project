using System.Text;
using System.Text.Json;
using ViewModels;
using static CommonFunctions.EnumUtility;

namespace AIChatService
{
    public class GeminiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiChatConfig _config;

        public GeminiChatService(HttpClient httpClient, GeminiChatConfig config)
        {
            _httpClient = httpClient;
            _config = config;

            _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
        }

        public async Task<string> SendMessageAsync(string userMessage, MessageType processMCPcatalog = MessageType.NormalChat, string mcpReponse = "")
        {
            var endpoint = $"models/{_config.GeminiChatModel}:generateContent?key={_config.GeminiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = processMCPcatalog == MessageType.MCPRequest ? userMessage + "\n" + MCPListingPrompt : 
                                         processMCPcatalog == MessageType.MCPResponse ? userMessage + "\n" + mcpReponse + "\n" + MCPResponsePrompt : 
                                         userMessage}
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error: {response.StatusCode} - {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<GeminiChatResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return responseData?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "[No response]";
        }

        private string MCPListingPrompt = @"
                                 You are a function router. Use ONLY the functions and parameters listed in the CATALOG. 
                                    Task:
                                    1) Pick exactly one function that best matches the USER REQUEST.
                                    2) Produce parameter values from the request.

                                    Output constraints (MUST follow exactly):
                                    - Output ONLY a single JSON object.
                                    - Do NOT echo the catalog or any prose.
                                    - JSON shape:
                                    {
                                      ""mcp"": {
                                        ""function"": {
                                          ""name"": ""<one function name from the catalog>"",
                                          ""parameters"": [
                                            { ""name"": ""<paramNameFromCatalog>"", ""value"": <JSON value or null> }
                                          ]
                                        }
                                      }
                                    }

                                    Rules:
                                    - Use only parameters that appear in the chosen function’s catalog entry.
                                    - If a required parameter value is missing, set ""value"": null.
                                    - Parse numbers/booleans from text when obvious (""true"", ""3.14"").
                                    - If nothing matches, return {""mcp"":{""function"":{""name"":""none"",""parameters"":[]}}}
                                    - NEVER include keys other than: mcp, function, name, parameters, name, value.
                                    - NEVER include the word ""Catalog"" or repeat the catalog content in your output.
                                    JSON_ONLY_START
                               ";

        private string MCPResponsePrompt = @"
                                 You are a helpful assistant. Use the information provided to answer the user.
                                    Task:
                                    1) Read the USER REQUEST and the FUNCTION RESPONSE.
                                    2) Provide a concise, clear answer to the user based on the FUNCTION RESPONSE.
                                    Output constraints (MUST follow exactly):
                                    - Output ONLY text that directly answers the USER REQUEST.
                                    - Do NOT include any JSON or technical details.
                                    - If the FUNCTION RESPONSE indicates an error or no data, respond with ""I'm sorry, I couldn't find any relevant information.""
                                    Rules:
                                    - Focus on clarity and relevance in your response.
                                    - Avoid repeating the USER REQUEST in your answer.
                                    - If the FUNCTION RESPONSE is empty or unhelpful, use the fallback response provided above.
                               ";
    }
}
