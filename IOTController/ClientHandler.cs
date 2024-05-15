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
        private StringBuilder messageBuilder;

        public ClientHandler(TcpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.stream = client.GetStream();
            this.messageBuilder = new StringBuilder();
        }

        public async Task<string> ReceiveMessageAsync()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        throw new Exception("Connection closed by remote host.");
                    }

                    string messagePart = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    messageBuilder.Append(messagePart);

                    // Check if the message is complete (assumes messages end with a newline)
                    if (messagePart.Contains("\n"))
                    {
                        string completeMessage = messageBuilder.ToString().Trim();
                        messageBuilder.Clear(); // Clear the buffer for the next message
                        return completeMessage;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ReceiveMessageAsync: {ex.Message}");
                    Dispose();
                    throw;
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                byte[] dataToSend = Encoding.ASCII.GetBytes(message + "\n"); // Ensure the message ends with a newline
                await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
                Console.WriteLine($"Sent: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendMessageAsync: {ex.Message}");
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            stream?.Close();
            client?.Close();
        }
    }
}
