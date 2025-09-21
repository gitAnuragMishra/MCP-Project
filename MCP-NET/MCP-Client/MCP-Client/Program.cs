using System.Diagnostics;
using AIChatService;
using MCP_Client.Mcp_Helper;
using MCPSharp;
using ViewModels;

var builder = WebApplication.CreateBuilder(args);

#region Add services and configurations
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration.GetSection("APIKeys").Get<GeminiChatConfig>() ?? new GeminiChatConfig();
config.GeminiChatModel = builder.Configuration["GeminiChatModel"];
builder.Services.AddSingleton(config);
builder.Services.AddHttpClient<GeminiChatService>();
builder.Services.AddScoped<ConnectServer>();

var mcpClient = new MCPClient(
   name: "McpClient",
   version: "v1.0.0",
   server: "E:\\MCP-Project\\MCP-NET\\MCP-Server\\MCPServer1\\bin\\Debug\\net8.0\\MCPServer1.exe");

builder.Services.AddSingleton(mcpClient);
#endregion


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

#region Cleanup running servers on shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    var processes = Process.GetProcessesByName("MCPServer1");
    foreach (var process in processes)
    {
        try
        {
            process.Kill(true);
            process.WaitForExit();
            Console.WriteLine($"Killed process {process.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error killing process {process.Id}: {ex.Message}");
        }
    }
});
#endregion

app.Run();
