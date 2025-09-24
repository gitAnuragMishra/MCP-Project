using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Core.Pipeline;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

internal class Program
{
    private static async Task Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        #region Configure MCP Client
        try
        {
            var httpOptions = new SseClientTransportOptions  // unified options type for HTTP/SSE
            {
                Endpoint = new Uri("http://localhost:5149/mcp"),
            };
            IClientTransport transport = new SseClientTransport(httpOptions);

            var clientOptions = new McpClientOptions
            {
                ClientInfo = new Implementation
                {
                    Name = "MCP-Client",
                    Version = "1.1.2"
                }
            };

            // Create and connect the MCP client using the transport
            IMcpClient mcpClient = await McpClientFactory.CreateAsync(transport, clientOptions);
            var tools = await mcpClient.ListToolsAsync();


            var arguments = new JsonObject
            {
                ["name"] = "get_string",
                ["arguments"] = new JsonObject
                {
                    ["i"] = 5
                }
            };


            /// Create the JSON-RPC 2.0 request for MCP tools
            var request = new JsonRpcRequest
            {
                Method = "tools/call",            /// MCP tool invocation method
                Params = arguments,               /// name and arguments
            };

            var response = await mcpClient.SendRequestAsync(request);




        }
        catch (Exception ex)
        {
            //
        }

        #endregion

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}