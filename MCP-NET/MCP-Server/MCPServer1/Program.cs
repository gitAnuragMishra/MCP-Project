using MCPSharp;
using Tools;
namespace MCPServer1;

internal class Program
{
    static async Task Main(string[] args)
    {
        MCPServer.Register<CalculatorTool>();
        MCPServer.Register<ProductTools>();

        await MCPServer.StartAsync(
            serverName: "EVA.McpServer",
            version: "v4.26");
    }
}