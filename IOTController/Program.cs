using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IOTController
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Start the ASP.NET Core host
            var host = CreateHostBuilder(args).Build();
            var greenhouseService = host.Services.GetRequiredService<GreenhouseService>();

            // Start the TCP server
            Server server = new Server();
            server.ClientConnected += async (TcpClient client) =>
            {
                ClientHandler clientHandler = new ClientHandler(client);
                greenhouseService.Initialize(clientHandler);
                GreenhouseManager greenhouseManager = new GreenhouseManager(clientHandler);

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


        private static async Task ProcessCommandsAsync(GreenhouseManager greenhouseManager)
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

                int id;
                if (!int.TryParse(parts[1], out id))
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
                    case "status":
                        var statusResult = await greenhouseManager.GetWindowStatus(id);
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
