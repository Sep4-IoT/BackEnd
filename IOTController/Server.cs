using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IOTController
{
    public class Server
    {
        private readonly int port = 50000;
        private TcpListener listener;
        private ClientHandler lastClientHandler;

        public event Func<ClientHandler, Task> ClientConnected;

        public Server()
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine($"Server started on port {port}. Waiting for client...");
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
                lastClientHandler = new ClientHandler(client); // Save the last client handler
                _ = HandleClientAsync(lastClientHandler); // Handle the client connection without blocking the acceptance loop
            }
        }

        private async Task HandleClientAsync(ClientHandler clientHandler)
        {
            try
            {
                if (ClientConnected != null)
                {
                    await ClientConnected(clientHandler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                clientHandler.Dispose();
                Console.WriteLine("Client disconnected.");
            }
        }

        public ClientHandler GetLastClientHandler()
        {
            return lastClientHandler;
        }
    }
}
