using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IOTController;

public class Controller
{
    private static byte[] data = null;
    private static bool sendData = false;
    private readonly IGreenHouseLogic greenHouseLogic;

    
    static async Task Main(string[] args)
    {
        await EstablishConnectionAsync();
    }
    
    static async Task EstablishConnectionAsync()
    {
        var listener = new TcpListener(IPAddress.Loopback, 5000);
        listener.Start();

        Console.WriteLine("Waiting for connection...");

        TcpClient client = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Connected!");

        await Task.WhenAll(
            ReceiveDataAsync(client.GetStream()),
            SendDataAsync(client.GetStream())
        );
            
        client.Close();
        listener.Stop();
    }
    
    static async Task ReceiveDataAsync(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                Console.WriteLine("Connection closed by remote host.");
                break;
            }
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            
            // Split the string by commas
            string[] parts = data.Split(',');

            // Create a List to store the substrings
            List<string> myList = new List<string>();

            // Add each substring to the List
            foreach (string part in parts)
            {
                myList.Add(part);
            }

            /*if (myList[0] == "WAR")
            {
                IGreenHouseLogic().sendWarningMessage(myList[1]);
            }*/

            if (myList[0] == "UPD")
            {
                if (myList[2] == "SER")
                {
                    IGreenHouseLogic().updateWindow(myList[3]);
                }
            }

        }
    }


    static void ChangeWindowStatus(int GreenHouseId, bool status)
    {
        string str = status ? "180" : "0";

        string message = "ACK" + GreenHouseId + "SER" + str;
        
        data = Encoding.ASCII.GetBytes(message);
        sendData = true;
    }
    
    static async Task SendDataAsync(NetworkStream stream)
    {
        while (true)
        {
            if(sendData){
            await stream.WriteAsync(data, 0, data.Length);
            sendData = false;
            }
        }
    }
    
    
}