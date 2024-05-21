using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOTController
{
    public class ClientHandler : IDisposable
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly Server server;

        public ClientHandler(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;
            this.stream = client.GetStream();
        }

        public async Task ProcessMessagesAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                await server.ProcessMessage(message);
            }
        }

        public async Task SendMessageAsync(string message)
        {
            byte[] dataToSend = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
            Console.WriteLine($"Sent: {message}");
        }

        public void Dispose()
        {
            stream.Close();
            client.Close();
        }
    }
}