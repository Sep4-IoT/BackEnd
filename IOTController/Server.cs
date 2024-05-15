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

        public event Func<TcpClient, Task> ClientConnected;

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
                _ = HandleClientAsync(client); // Handle the client connection without blocking the acceptance loop
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                if (ClientConnected != null)
                {
                    await ClientConnected(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}