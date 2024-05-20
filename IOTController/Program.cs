using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Application.LogicInterfaces;

namespace IOTController
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var greenhouseService = host.Services.GetRequiredService<GreenhouseService>();
            var greenhouseLogic = host.Services.GetRequiredService<IGreenHouseLogic>();

            // Start the TCP server
            var server = new Server();
            server.ClientConnected += async (TcpClient client) =>
            {
                var clientHandler = new ClientHandler(client);
                greenhouseService.Initialize(clientHandler);
                var greenhouseManager = new GreenHouseManager(clientHandler, greenhouseLogic);

                Console.WriteLine("Client connected.");
                await ProcessCommandsAsync(greenhouseManager);
            };

            _ = server.StartAsync();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:6000");
                });

        private static async Task ProcessCommandsAsync(GreenHouseManager greenhouseManager)
        {
            
            Console.WriteLine("Server is running. Type 'open', 'close', 'status', or 'set [id] [angle]' followed by the greenhouse ID and optionally an angle to control:");

            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                var parts = input.Split();
                if (parts.Length < 2)
                {
                    Console.WriteLine("Invalid command. Please use the format 'open [id]', 'close [id]', 'status [id]', or 'set [id] [angle]'.");
                    continue;
                }

                if (!int.TryParse(parts[1], out var id))
                {
                    Console.WriteLine("Invalid greenhouse ID. Please enter a valid number.");
                    continue;
                }

                switch (parts[0].ToLower())
                {
                    case "open":
                        await greenhouseManager.OpenWindow(id);
                        break;
                    case "close":
                        await greenhouseManager.CloseWindow(id);
                        break;
                    case "temperature":
                        await greenhouseManager.GetTemperature(id);
                        break;
                    case "status":
                        var statusResult = await greenhouseManager.GetWindowStatus(id);
                        if (statusResult.ErrorMessage != null)
                        {
                            Console.WriteLine($"Error retrieving window status for Greenhouse {id}: {statusResult.ErrorMessage}");
                        }
                        else
                        {
                            var statusDescription = statusResult.IsWindowOpen.HasValue
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
                        if (!int.TryParse(parts[2], out var angle))
                        {
                            Console.WriteLine("Invalid angle. Please enter a valid number between 0 and 180.");
                            continue;
                        }
                        await greenhouseManager.SetWindowAngle(id, angle);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Use 'open', 'close', 'status', or 'set'.");
                        break;
                }
            }
        }
    }
}
