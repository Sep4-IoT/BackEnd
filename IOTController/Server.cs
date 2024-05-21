using System.Net;
using System.Net.Sockets;

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
                if (ClientConnected != null)
                {
                    await ClientConnected(client);
                }
            }
        }
    }
}