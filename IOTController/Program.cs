using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IOTController;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var server = host.Services.GetRequiredService<Server>();

        // Run the TCP server in a background task
        var serverTask = server.StartAsync();
        
        var greenhouseService = host.Services.GetRequiredService<GreenhouseService>();
        greenhouseService.Initialize(server);

        // Run the HTTP server
        var webHostTask = host.RunAsync();

        await Task.WhenAll(serverTask, webHostTask);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ListenAnyIP(6000); // HTTP port
                    // Comment out HTTPS if not configured
                    // serverOptions.ListenAnyIP(6001, listenOptions =>
                    // {
                    //     listenOptions.UseHttps();
                    // });
                });
            });
}