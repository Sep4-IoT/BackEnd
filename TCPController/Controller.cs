using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPController
{
    public class Controller
    {
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream stream;
        private readonly int port = 5000;

        public Controller()
        {
            listener = new TcpListener(IPAddress.Any, port);
            try
            {
                listener.Start();
                Console.WriteLine($"Server started on port {port}. Waiting for client...");
                AcceptClient();
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Could not start server on port {port}. Error: {ex.Message}");
                // Optionally handle exit or retry here
            }
        }

        private async void AcceptClient()
        {
            try
            {
                client = await listener.AcceptTcpClientAsync();
                stream = client.GetStream();
                Console.WriteLine("Client connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
        
        private async Task<string> ReceiveResponseAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                throw new Exception("Connection closed by remote host.");
            }
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        public async Task OpenWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,180";
            await SendMessageAsync(message);
        }

        public async Task CloseWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,0";
            await SendMessageAsync(message);
        }
        
        public async Task<bool> GetWindowStatus(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},GET,SER";
            await SendMessageAsync(message);

            // Wait to receive a response and interpret it
            string response = await ReceiveResponseAsync();

            // Log the received response for debugging purposes
            Console.WriteLine("Received: " + response);
            
            // Split and check the response format
            string[] parts = response.Split(',');
            
            if (parts.Length < 5)  // Check if all expected parts are present
            {
                Console.WriteLine("Received malformed response: " + response);
                throw new InvalidOperationException("Received malformed response.");
            }

            // Specific position checks for robustness
            if (parts[0] == "ACK" && parts[1] == GreenHouseId.ToString() && parts[2] == "GET" && parts[3] == "SER")
            {
                if (parts[4] == "180") // Assuming "180" means open
                {
                    return true; // Window is open
                }
                if (parts[4] == "0") // Assuming "0" means closed
                {
                    return false; // Window is closed
                }
            }

            Console.WriteLine("Received unexpected status: " + parts[4]);
            throw new InvalidOperationException("Invalid status value received.");
        }


        private async Task SendMessageAsync(string message)
        {
            if (client != null && stream != null)
            {
                byte[] dataToSend = Encoding.ASCII.GetBytes(message);
                await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
                Console.WriteLine($"Sent: {message}");
            }
            else
            {
                Console.WriteLine("No client connected to send the message.");
            }
        }

        public static async Task Main(string[] args)
        {
            Controller server = new Controller();
            Console.WriteLine("Server is running. Type 'open', 'close', or 'status' followed by the greenhouse ID to control:");

            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                string[] parts = input.Split();
                if (parts.Length != 2)
                {
                    Console.WriteLine("Invalid command. Please use the format 'open [id]', 'close [id]', or 'status [id]'.");
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
                        await server.OpenWindow(id);
                        break;
                    case "close":
                        await server.CloseWindow(id);
                        break;
                    case "status":
                        bool isOpen = await server.GetWindowStatus(id);
                        Console.WriteLine($"Window status for Greenhouse {id}: {(isOpen ? "Open" : "Closed")}");
                        break;
                    default:
                        Console.WriteLine("Unknown command. Use 'open', 'close', or 'status'.");
                        break;
                }
            }
        }
    }
}
