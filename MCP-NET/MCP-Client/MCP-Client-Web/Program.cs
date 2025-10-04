using System.Text.Json.Nodes;
using AIChatService;
using CommonFunctions;
using ModelContextProtocol.Protocol;
using ViewModels;

internal class Program
{
    private static async Task Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<ICustomMcpClientFactory, CustomMcpClientFactory>();
        var config = builder.Configuration.GetSection("APIKeys").Get<GeminiChatConfig>() ?? new GeminiChatConfig();
        config.GeminiChatModel = builder.Configuration["GeminiChatModel"];
        builder.Services.AddSingleton(config);
        builder.Services.AddHttpClient<GeminiChatServiceWeb>();
        builder.Services.AddScoped<ConnectServerHttp>();

        //////////////////////////////////////
#if DEBUG

        //var _clientFactory = new CustomMcpClientFactory();
        //var mcpClient = await _clientFactory.CreateAsync();
        //var tools = await mcpClient.ListToolsAsync();

        //var arguments = new JsonObject
        //{
        //    ["name"] = "get_products",
        //    ["arguments"] = new JsonObject
        //    {
        //        ["i"] = 1
        //    }
        //};

        ///// Create the JSON-RPC 2.0 request for MCP tools
        //var request = new JsonRpcRequest
        //{
        //    Method = "tools/call",            /// MCP tool invocation method
        //    Params = arguments,               /// name and arguments
        //};

        //var response = await mcpClient.SendRequestAsync(request);

#endif
        ///////////////////////////////////////


        var app = builder.Build();

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