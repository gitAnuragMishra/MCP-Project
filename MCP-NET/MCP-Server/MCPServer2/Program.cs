using MCPServer1.Tools;
using Microsoft.Extensions.Options;
using Tools;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();
        builder.Services.AddHttpClient();
        var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        builder.Services.Configure<AppConfig>(conf.GetSection("AppConfig"));
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppConfig>>().Value);
        builder.Services.AddScoped<HttpUtility>();

        //builder.Services.AddControllers();
        //builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}

        //app.UseHttpsRedirection();

        //app.UseAuthorization();

        //app.MapControllers();

        app.MapMcp("/mcp");

        app.Run();


//#if DEBUG
//        var session = new SessionTool();
//        var userID = session.StartSession(0, "anurag");
//#endif
    }

    //public static class ServiceLocator
    //{
    //    public static IServiceProvider ServiceProvider { get; set; }
    //}
}

public class AppConfig
{
    public string FastAPIURL { get; set; }
    public string ConnectionString { get; set; }
    public int NRequest { get; set; }
}