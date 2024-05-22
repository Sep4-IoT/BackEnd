using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine("Client connected.");
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

        // Process the message received from the iot device and send it to the database
        public async Task ProcessMessage(string message)
        {
            string[] parts = message.Split(',');

            if (parts.Length < 4)
            {
                Console.WriteLine("Received malformed message: " + message);
                return;
            }

            string messageType = parts[0];
            string greenhouseId = parts[1];

            Console.WriteLine("I am handling the message: " + message);
            switch (messageType)
            {
                case "ACK":
                    Console.WriteLine("looks like it is an ack message:");
                    if (parts[2] == "GET" && parts[3] == "SER")
                    {
                        if (parts[4].Trim().Equals("180", StringComparison.InvariantCulture))
                        {
                            await SendWindowOpened(greenhouseId);
                        }
                        else if (parts[4].Trim().Equals("0", StringComparison.InvariantCulture))
                        {
                            await SendWindowClosed(greenhouseId);
                        }
                        Console.WriteLine("I got problems comparing strings:" + parts[4]);
                    }
                    else
                    {
                        Console.WriteLine("Received unexpected ACK message format: " + message);
                    }
                    break;

                case "UPD":
                    Console.WriteLine("looks like it is an update message:");
                    if (parts[2] == "SER")
                    {
                        if (parts[4].Trim().Equals("180", StringComparison.InvariantCulture))
                        {
                            await SendWindowOpened(greenhouseId);
                        }
                        else if (parts[4].Trim().Equals("0", StringComparison.InvariantCulture))
                        {
                            await SendWindowClosed(greenhouseId);
                        }
                        Console.WriteLine("I got problems comparing strings:" + parts[4]);
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

        // methods for postasync to the database
        private async Task SendWindowOpened(string greenhouseId)
        {
            Console.WriteLine("sending window opened to database");
            var requestUri = $"/GreenHouse/{greenhouseId}/openWindow";
            await _dbApiClient.PostAsync(requestUri, null);
        }

        private async Task SendWindowClosed(string greenhouseId)
        {
            Console.WriteLine("sending window closed to database");
            var requestUri = $"/GreenHouse/{greenhouseId}/closeWindow";
            await _dbApiClient.PostAsync(requestUri, null);
        }
    }
}
