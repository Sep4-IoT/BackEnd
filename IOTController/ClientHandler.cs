using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOTController
{
    public class ClientHandler
    {
        private TcpClient client;
        private NetworkStream stream;

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
        }

        public async Task<string> ReceiveMessageAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                throw new Exception("Connection closed by remote host.");
            }
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        public async Task SendMessageAsync(string message)
        {
            byte[] dataToSend = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
            Console.WriteLine($"Sent: {message}");
        }
    }
}