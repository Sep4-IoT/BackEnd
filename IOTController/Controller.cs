using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOTController
{
    public class Controller
    {
        private static Controller instance = null;
        private static readonly object padlock = new object();

        public static Controller Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Controller();
                    }
                    return instance;
                }
            }
        }

        private TcpListener listener;
        private TcpClient client;
        private NetworkStream stream;
        private readonly int port = 50000;

        private Controller()
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
        
        public class WindowStatusResult
        {
            public bool? IsWindowOpen { get; set; }
            public string ErrorMessage { get; set; }

            public WindowStatusResult(bool? isWindowOpen, string errorMessage = null)
            {
                IsWindowOpen = isWindowOpen;
                ErrorMessage = errorMessage;
            }
        }

        public async Task<WindowStatusResult> GetWindowStatus(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},GET,SER";
            await SendMessageAsync(message);

            // Wait to receive a response and interpret it
            string response = await ReceiveResponseAsync();

            // Log the received response for debugging purposes
            Console.WriteLine("Received: " + response);
    
            // Split and check the response format
            string[] parts = response.Split(',');
    
            if (parts.Length < 4)  // Check if all expected parts are present
            {
                Console.WriteLine("Received malformed response: " + response);
                return new WindowStatusResult(null, "Received malformed response.");
            }

            // Specific position checks for robustness
            if (parts[0] == "ACK" && parts[1] == GreenHouseId.ToString() && parts[2] == "GET" && parts[3] == "SER")
            {
                if (parts[4] == "180") // Assuming "180" means open
                {
                    return new WindowStatusResult(true); // Window is open
                }
                if (parts[4] == "0") // Assuming "0" means closed
                {
                    return new WindowStatusResult(false); // Window is closed
                }
            }

            Console.WriteLine("Received unexpected status: " + parts[4]);
            return new WindowStatusResult(null, $"Invalid status value received: {parts[4]}.");
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
                        await server.OpenWindow(id);
                        break;
                    case "close":
                        await server.CloseWindow(id);
                        break;
                    case "status":
                        var statusResult = await server.GetWindowStatus(id);
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
                        await server.SetWindowAngle(id, angle);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Use 'open', 'close', 'status', or 'set'.");
                        break;
                }
            }
        }
        public async Task SetWindowAngle(int GreenHouseId, int angle)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,{angle}";
            await SendMessageAsync(message);
        }


    }
}