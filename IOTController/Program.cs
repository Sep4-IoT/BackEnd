﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Application.LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IOTController
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Ensure the database is created
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<GreenHouseContext>();
                context.Database.Migrate();
                context.Database.EnsureCreated();
            }

            var greenhouseService = host.Services.GetRequiredService<GreenHouseService>();

            // Start the TCP server
            var server = new Server();
            server.ClientConnected += async (TcpClient client) =>
            {
                try
                {
                    var clientHandler = new ClientHandler(client);
                    greenhouseService.Initialize(clientHandler);

                    Console.WriteLine("Client connected.");
                    await ProcessCommandsAsync(greenhouseService);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
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

        private static async Task ProcessCommandsAsync(GreenHouseService greenHouseService)
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
                        await greenHouseService.OpenWindow(id);
                        break;
                    case "close":
                        await greenHouseService.CloseWindow(id);
                        break;
                    case "temperature":
                        await greenHouseService.GetTemperature(id);
                        break;
                    case "status":
                        var statusResult = await greenHouseService.GetWindowStatus(id);
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
                        await greenHouseService.SetWindowAngle(id, angle);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Use 'open', 'close', 'status', or 'set'.");
                        break;
                }
            }
        }
    }
}
