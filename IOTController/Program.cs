using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IOTController
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var server = new Server();
            var greenhouseService = new GreenhouseService(server);

            server.ClientConnected += async (clientHandler) =>
            {
                Console.WriteLine("New client handler received.");
                if (greenhouseService.IsInitialized)
                {
                    Console.WriteLine("Reinitializing GreenhouseService with new ClientHandler.");
                }
                greenhouseService.Initialize(clientHandler);

                Console.WriteLine("Client connected.");
                await ProcessCommandsAsync(greenhouseService);
            };

            _ = server.StartAsync();

            var lastClientHandler = server.GetLastClientHandler();
            if (lastClientHandler != null)
            {
                greenhouseService.Initialize(lastClientHandler);
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:6000");
                });

        private static async Task ProcessCommandsAsync(GreenhouseService greenhouseService)
        {
            Console.WriteLine("Server is running. Type 'open', 'close', 'status', or 'set [id] [angle]' followed by the greenhouse ID and optionally an angle to control:");

            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                string[] parts = input.Split();
                if (parts.Length < 2)
                {
                    Console.WriteLine("Invalid command. Please use the format 'open [id]', 'close [id]', 'status [id]', or 'set [id] [angle]'.");
                    continue;
                }

                if (!int.TryParse(parts[1], out int id))
                {
                    Console.WriteLine("Invalid greenhouse ID. Please enter a valid number.");
                    continue;
                }

                switch (parts[0].ToLower())
                {
                    case "open":
                        await greenhouseService.OpenWindow(id);
                        break;
                    case "close":
                        await greenhouseService.CloseWindow(id);
                        break;
                    case "status":
                        var statusResult = await greenhouseService.GetWindowStatus(id);
                        if (statusResult.ErrorMessage != null)
                        {
                            Console.WriteLine($"Error retrieving window status for Greenhouse {id}: {statusResult.ErrorMessage}");
                        }
                        else
                        {
                            string statusDescription = statusResult.IsWindowOpen.HasValue
                                ? (statusResult.IsWindowOpen.Value ? "Open" : "Closed")
                                : "Status not determined";
                            Console.WriteLine($"Window status for Greenhouse {id}: {statusDescription}");
                        }
                        break;
                    case "set":
                        if (parts.Length != 3)
                        {
                            Console.WriteLine("Invalid command. Use the format 'set [id] [angle]'.");
                            continue;
                        }
                        if (!int.TryParse(parts[2], out int angle))
                        {
                            Console.WriteLine("Invalid angle. Please enter a valid number between 0 and 180.");
                            continue;
                        }
                        await greenhouseService.SetWindowAngle(id, angle);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Use 'open', 'close', 'status', or 'set'.");
                        break;
                }
            }
        }
    }
}
