using MCPSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tools;


namespace MCPServer1;

internal class Program
{
    static async Task Main(string[] args)
    {
        #region Configuration and Dependancies
        var services = new ServiceCollection();
        services.AddHttpClient(); //For http client factory
        //services.AddHttpContextAccessor();

        var conf = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

        services.Configure<AppConfig>(conf.GetSection("AppConfig"));
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppConfig>>().Value);

        /// using service locator method to inject dependancies
        var serviceProvider = services.BuildServiceProvider();
        ServiceLocator.ServiceProvider = serviceProvider;


        services.AddScoped<HttpUtility>();
        #endregion

#if DEBUG
        ////////////////////////////////////////////////////
        //var tools = new ProductsTool();
        //var res = await tools.GetProducts(1);
        //var buyres = await tools.BuyProduct(1, 2, 1);
        //var res1 = await tools.GetProducts(1);

        ////////////////////////////////////////////////////
#endif

        #region MCP Tool registration
        //  MCP tool registration
        MCPServer.Register<CalculatorTool>();
        MCPServer.Register<ProductsTool>();

        // Blocking (async)
        await Task.Run(() => MCPServer.StartAsync(
            serverName: "EVA.McpServer",
            version: "v1.0.0"));

        //await Task.Delay(-1);
        #endregion
    }

    /// <summary>
    /// Need a static service provider as we can't inject custom dependancies in MCP Tools through registration
    /// </summary>
    public static class ServiceLocator
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }

    public class AppConfig
    {
        public string FastAPIURL { get; set; }
    }
}
