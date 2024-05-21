using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace IOTController
{
    public class Server
    {
        private readonly int port = 50000;
        private TcpListener listener;
        private readonly HttpClient _dbApiClient;
        private readonly List<ClientHandler> clients = new List<ClientHandler>();

        public Server(HttpClient dbApiClient)
        {
            _dbApiClient = dbApiClient;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine($"Server started on port {port}. Waiting for clients...");
                await AcceptClientsAsync();
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Could not start server on port {port}. Error: {ex.Message}");
            }
        }

        private async Task AcceptClientsAsync()
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                var clientHandler = new ClientHandler(client, this);
                clients.Add(clientHandler);
                _ = HandleClientAsync(clientHandler); // Fire-and-forget to handle clients concurrently
            }
        }

        private async Task HandleClientAsync(ClientHandler clientHandler)
        {
            try
            {
                await clientHandler.ProcessMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with client: {ex.Message}");
            }
            finally
            {
                clients.Remove(clientHandler);
                clientHandler.Dispose();
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            List<Task> sendTasks = new List<Task>();
            foreach (var client in clients)
            {
                sendTasks.Add(client.SendMessageAsync(message));
            }
            await Task.WhenAll(sendTasks);
        }

        public async Task ProcessMessage(string message)
        {
            string[] parts = message.Split(',');

            if (parts.Length < 4)
            {
                Console.WriteLine("Received malformed message: " + message);
                return;
            }

            string messageType = parts[0];
            int greenhouseId;
            bool? isWindowOpen = null;

            if (!int.TryParse(parts[1], out greenhouseId))
            {
                Console.WriteLine("Invalid greenhouse ID: " + parts[1]);
                return;
            }

            switch (messageType)
            {
                case "ACK":
                    if (parts[2] == "GET" && parts[3] == "SER")
                    {
                        if (parts[4] == "180") await SendWindowOpened(greenhouseId);
                        else if (parts[4] == "0") await SendWindowClosed(greenhouseId);
                    }
                    else
                    {
                        Console.WriteLine("Received unexpected ACK message format: " + message);
                    }
                    break;

                case "UPD":
                    if (parts.Length >= 4 && parts[2] == "SER")
                    {
                        if (parts[4] == "180") await SendWindowOpened(greenhouseId);
                        else if (parts[4] == "0") await SendWindowClosed(greenhouseId);
                    }
                    else
                    {
                        Console.WriteLine("Received unexpected UPD message format: " + message);
                    }
                    break;

                default:
                    Console.WriteLine("Received unknown message type: " + messageType);
                    break;
            }
        }

        private bool? ParseWindowStatus(string status)
        {
            return status switch
            {
                "180" => true,
                "0" => false,
                _ => null,
            };
        }

        private async Task SendWindowOpened(int greenhouseId)
        {
            Console.WriteLine("Sending data to database...");

            var requestUri = $"/GreenHouse/{greenhouseId}/openWindow";
            await _dbApiClient.PostAsync(requestUri, null);
        }
        
        private async Task SendWindowClosed(int greenhouseId)
        {
            Console.WriteLine("Sending data to database...");

            var requestUri = $"/GreenHouse/{greenhouseId}/closeWindow";
            await _dbApiClient.PostAsync(requestUri, null);
        }
    }
}
