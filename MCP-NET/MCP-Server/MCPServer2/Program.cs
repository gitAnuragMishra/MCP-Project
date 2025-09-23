using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using ModelContextProtocol.Server;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        /// try to use DI here
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        ServiceLocator.ServiceProvider = serviceProvider;



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

        app.MapMcp();

        app.Run();


        
    }

    public static class ServiceLocator
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }


    [McpServerToolType]
    public static class MyClass
    {

        [McpServerTool, Description("This converts int to string")]
        public static string GetString ([Description("Integer input")]  int i)
        {
            return i.ToString();
        }

        [McpServerTool, Description("This converts list of int to string")]
        public static string GetStringFromList([Description("List of integer as input")] List<int> i)
        {
            return string.Join(", ", i);
        }
    }
}