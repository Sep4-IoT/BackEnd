using TCPController;
using System;
using System.Threading.Tasks;
using Application.Logic;
using Application.LogicInterfaces;

namespace Tester
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IArduinoLogic server = new ArduinoLogic();
            
            Console.WriteLine("Testing the server. Please enter the greenhouse ID to test:");
            int greenhouseId = Convert.ToInt32(Console.ReadLine());
            
            Console.WriteLine($"Attempting to open window for Greenhouse {greenhouseId}...");
            await server.OpenWindow(greenhouseId);
            Console.WriteLine("Open command sent.");
            
            Console.WriteLine($"Attempting to close window for Greenhouse {greenhouseId}...");
            await server.CloseWindow(greenhouseId);
            Console.WriteLine("Close command sent.");
            
            Console.WriteLine($"Checking status for Greenhouse {greenhouseId}...");
            bool isOpen = await server.GetWindowStatus(greenhouseId);
            Console.WriteLine($"Window status for Greenhouse {greenhouseId}: {(isOpen ? "Open" : "Closed")}");
        }
    }
}