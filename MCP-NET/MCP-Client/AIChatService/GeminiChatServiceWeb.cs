using System.Text;
using System.Text.Json;
using ViewModels;
using static CommonFunctions.EnumUtility;

namespace AIChatService
{
    public class GeminiChatServiceWeb
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiChatConfig _config;

        public GeminiChatServiceWeb(HttpClient httpClient, GeminiChatConfig config)
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
                                         processMCPcatalog == MessageType.MCPResponse ? "The USER query asked: " + userMessage + ". The RESPONSE: " + mcpReponse + ". " + MCPResponsePrompt :
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
                                            { ""name"": ""<paramNameFromCatalog>"", ""Value"": <JSON value or null> }
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
                                    - If the parameter description say exactly PARAMETERLESS, then set ""value"": 1.
                                    JSON_ONLY_START
                               ";

        private string MCPResponsePrompt = @"
                                 You are a helpful assistant responding to a user's question. You are given:
                                    - The USER REQUEST (what they asked).
                                    - The RESPONSE (the structured output generated from a system function).

                                    Your job is to combine both into a natural, friendly response that directly helps the user.

                                    Instructions:
                                    1) Read and understand the USER REQUEST.
                                    2) Interpret the FUNCTION RESPONSE.
                                    3) Write a short, clear, and friendly answer that feels natural — like a real human helping.

                                    Output constraints (MUST follow exactly):
                                    - ONLY output the final response to the user. No JSON, no code, no system explanations.

                                    Tone and Style:
                                    - Be polite, helpful, and conversational.
                                    - Avoid repeating the full USER REQUEST or FUNCTION RESPONSE.
                                    - If numerical or specific data is available, summarize it clearly.
                                    - Don't make assumptions beyond the given information.
                                    ";
    }
}
