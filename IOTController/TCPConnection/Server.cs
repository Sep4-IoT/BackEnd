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
        private readonly int port = 50001;
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
            
            
            
            
            
            

            if (parts.Length < 3)
            {
                Console.WriteLine("Received malformed message: " + message);
                return;
            }

            string messageType = parts[0];
            string greenhouseId = parts[1];

            Console.WriteLine("I am handling the message: " + message);
            switch (messageType)
            {
                //RES,1,SER,180
                case "RES":
                    Console.WriteLine("looks like it is an ack message:");
                    if (parts[2] == "SER")
                    {
                        string numberResponse = parts[3] ;
                        string processedString = TruncateAfterNumbers(numberResponse);
                        if (processedString.Trim().Equals("180", StringComparison.InvariantCulture))
                        {
                            await SendWindowOpened(greenhouseId);
                        }
                        else if (processedString.Trim().Equals("0", StringComparison.InvariantCulture))
                        {
                            await SendWindowClosed(greenhouseId);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Received unexpected ACK message format: " + message);
                    }
                    break;

                case "UPD":
                    Console.WriteLine("looks like it is an update message:");
                    if (parts[3] == "TEM")
                    {
                        string numberResponse = parts[4] ;
                        string processedString = TruncateAfterNumbers(numberResponse);
                        var temperature = processedString.Trim();
                        var requestUri = $"/GreenHouse/{greenhouseId}/updateTemperature/{temperature}";
                        await _dbApiClient.PostAsync(requestUri, null);
                    }
                    else if (parts[3] == "HUM")
                    {
                        string numberResponse = parts[4] ;
                        string processedString = TruncateAfterNumbers(numberResponse);
                        var humidity = processedString.Trim();
                        var requestUri = $"/GreenHouse/{greenhouseId}/updateHumidity/{humidity}";
                        
                        
                        await _dbApiClient.PostAsync(requestUri, null);
                    }
                    else if (parts[3] == "LIG")
                    {
                        string numberResponse = parts[4] ;
                        string processedString = TruncateAfterNumbers(numberResponse);
                        var light = processedString.Trim();
                        var requestUri = $"/GreenHouse/{greenhouseId}/updateLight/{light}";
                        await _dbApiClient.PostAsync(requestUri, null);
                    }
                    else if (parts[3] == "CO2")
                    {
                        string numberResponse = parts[4] ;
                        string processedString = TruncateAfterNumbers(numberResponse);
                        var light = processedString.Trim();
                        var requestUri = $"/GreenHouse/{greenhouseId}/updateCO2/{light}";
                        await _dbApiClient.PostAsync(requestUri, null);
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
        
        public static string TruncateAfterNumbers(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input; // Return the input if it's null or empty
            }

            int index = 0;

            // Iterate through each character in the string
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    break; // Stop when we find the first non-numeric character
                }
                index++;
            }

            // Truncate the string up to the point where we found the first non-numeric character
            return input.Substring(0, index);
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
