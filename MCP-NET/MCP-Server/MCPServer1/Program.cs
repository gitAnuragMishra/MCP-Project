using System.Runtime.Loader;
using MCPSharp;
using Tools;

namespace MCPServer1;

internal class Program
{
    static async Task Main(string[] args)
    {
        var exitEvent = new TaskCompletionSource();

        // Hook shutdown signals (Ctrl+C, Docker stop, etc.)
        AssemblyLoadContext.Default.Unloading += _ =>
        {
            Console.WriteLine("SIGTERM received.");
            exitEvent.TrySetResult();
        };

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Ctrl+C pressed.");
            e.Cancel = true; // prevent immediate termination
            exitEvent.TrySetResult();
        };

        // Register your tools
        MCPServer.Register<CalculatorTool>();
        MCPServer.Register<ProductTools>();

        // Run the server in the background (since it blocks forever)
        _ = Task.Run(() => MCPServer.StartAsync(
            serverName: "EVA.McpServer",
            version: "v4.26"));

        Console.WriteLine("MCP Server started. Press Ctrl+C to exit.");

        // Wait for exit signal
        await exitEvent.Task;

        Console.WriteLine("Shutting down...");
        Environment.Exit(0); // kill the app
    }
}
