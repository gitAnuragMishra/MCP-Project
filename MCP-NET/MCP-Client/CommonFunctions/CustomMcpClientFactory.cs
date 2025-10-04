using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace CommonFunctions
{
    public interface ICustomMcpClientFactory
    {
        Task<McpClient> CreateAsync();
    }

    public class CustomMcpClientFactory : ICustomMcpClientFactory
    {
        /// <summary>
        /// Returns a fresh McpClient on each call of CreateAsync, safe of AddSingleton()
        /// </summary>
        public async Task<McpClient> CreateAsync()
        {
            var httpOptions = new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://localhost:7025/mcp")
            };

            IClientTransport transport = new HttpClientTransport(httpOptions);

            var clientOptions = new McpClientOptions
            {
                ClientInfo = new Implementation
                {
                    Name = "MCP-Client",
                    Version = "1.2.0"
                }
            };

            return await McpClient.CreateAsync(transport, clientOptions);
        }
    }
}
