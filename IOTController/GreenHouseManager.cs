using Application.LogicInterfaces;
using System;
using System.Threading.Tasks;

namespace IOTController
{
    public class GreenHouseManager
    {
        private readonly ClientHandler clientHandler;
        private readonly IGreenHouseLogic greenHouseLogic;

        public GreenHouseManager(ClientHandler clientHandler, IGreenHouseLogic greenHouseLogic)
        {
            this.clientHandler = clientHandler;
            this.greenHouseLogic = greenHouseLogic;
        }

        public async Task<string> OpenWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,180";
            await clientHandler.SendMessageAsync(message);
            return await clientHandler.ReceiveMessageAsync();
        }

        public async Task<string> CloseWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,0";
            await clientHandler.SendMessageAsync(message);
            return await clientHandler.ReceiveMessageAsync();
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
            await clientHandler.SendMessageAsync(message);

            string response = await clientHandler.ReceiveMessageAsync();
            Console.WriteLine("Received: " + response);

            string[] parts = response.Split(',');
            if (parts.Length < 4)
            {
                Console.WriteLine("Received malformed response: " + response);
                return new WindowStatusResult(null, "Received malformed response.");
            }

            if (parts[0] == "ACK" && parts[1] == GreenHouseId.ToString() && parts[2] == "GET" && parts[3] == "SER")
            {
                if (parts[4] == "180")
                {
                    return new WindowStatusResult(true);
                }
                if (parts[4] == "0")
                {
                    return new WindowStatusResult(false);
                }
            }

            Console.WriteLine("Received unexpected status: " + parts[4]);
            return new WindowStatusResult(null, $"Invalid status value received: {parts[4]}.");
        }

        public async Task SetWindowAngle(int greenHouseId, int angle)
        {
            string message = $"REQ,{greenHouseId},SET,SER,{angle}";
            await clientHandler.SendMessageAsync(message);
        }

        public class TemperatureResult
        {
            public double? Temperature { get; set; }
            public string ErrorMessage { get; set; }

            public TemperatureResult(double? temperature, string errorMessage = null)
            {
                Temperature = temperature;
                ErrorMessage = errorMessage;
            }
        }

        public async Task<TemperatureResult> GetTemperature(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},GET,TEM";
            await clientHandler.SendMessageAsync(message);

            string response = await clientHandler.ReceiveMessageAsync();
            Console.WriteLine("Received: " + response);

            string[] parts = response.Split(',');
            if (parts.Length < 4)
            {
                Console.WriteLine("Received malformed response: " + response);
                return new TemperatureResult(null, "Received malformed response.");
            }

            if (double.TryParse(parts[4], out double temperature))
            {
                try
                {
                    await greenHouseLogic.UpdateTemperature(GreenHouseId, temperature);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating temperature: " + ex.Message);
                    return new TemperatureResult(null, "Error updating temperature: " + ex.Message);
                }
                return new TemperatureResult(temperature);
            }
            else
            {
                Console.WriteLine("Received unexpected status: " + parts[4]);
                return new TemperatureResult(null, $"Invalid status value received: {parts[4]}.");
            }
        }
    }
}
