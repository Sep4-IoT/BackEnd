using System.Net;
using System.Net.Sockets;
using System.Text;
using Application.LogicInterfaces;

namespace ArduinoController;

public class Controller
{
    private static byte[] dataToSend;
    private static bool sendData;
    private static string receivedData;
    
    static async Task Main(string[] args)
    {
        await EstablishConnectionAsync();
    }
    
    static async Task EstablishConnectionAsync()
    {
        var listener = new TcpListener(IPAddress.Any, 5000);
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
            
            receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
    }

    static void OpenWindow(int GreenHouseId)
    {
        string message = "REQ" + "," + GreenHouseId + "," + "SET" + "SER" + "," + "180";
        dataToSend = Encoding.ASCII.GetBytes(message);
        sendData = true;
    }

    static void CloseWindow(int GreenHouseId)
    {
        string message = "REQ" + "," + GreenHouseId + "," + "SET" + "SER" + "," + "0";
        dataToSend = Encoding.ASCII.GetBytes(message);
        sendData = true;
    }

    static bool GetWindowStatus(int GreenHouseId)
    {
        
        string message = "REQ" + "," + GreenHouseId + "," + "GET" + "SER";
        dataToSend = Encoding.ASCII.GetBytes(message);
        sendData = true;

        Thread.Sleep(500);

        string[] parts = receivedData.Split(",");

        if (parts[4] == "180")
        {
            return true;
        }
        return false;
    }

    static async Task SendDataAsync(NetworkStream stream)
    {
    
        while (true)
        {
            /*
            string number = Console.ReadLine();

            // Construct the message
            string message = "REQ,10,SET,SER," + number;

            // Encode and send the message
            byte[] data = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);

            string message = "REQ,10,GET,SER";
            byte[] data = Encoding.ASCII.GetBytes(message);
            */
            if (sendData){
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
            sendData = false;
            }
            

        }
    }

    
    
}