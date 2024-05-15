using System.Threading.Tasks;

namespace IOTController
{
    public class GreenhouseManager
    {
        private ClientHandler clientHandler;

        public GreenhouseManager(ClientHandler clientHandler)
        {
            this.clientHandler = clientHandler;
        }

        public async Task OpenWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,180";
            await clientHandler.SendMessageAsync(message);
        }

        public async Task CloseWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,0";
            await clientHandler.SendMessageAsync(message);
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

            // Wait to receive a response and interpret it
            string response = await clientHandler.ReceiveMessageAsync();

            // Log the received response for debugging purposes
            Console.WriteLine("Received: " + response);

            // Split and check the response format
            string[] parts = response.Split(',');

            if (parts.Length < 4)  // Check if all expected parts are present
            {
                Console.WriteLine("Received malformed response: " + response);
                return new WindowStatusResult(null, "Received malformed response.");
            }

            // Specific position checks for robustness
            if (parts[0] == "ACK" && parts[1] == GreenHouseId.ToString() && parts[2] == "GET" && parts[3] == "SER")
            {
                if (parts[4] == "180") // Assuming "180" means open
                {
                    return new WindowStatusResult(true); // Window is open
                }
                if (parts[4] == "0") // Assuming "0" means closed
                {
                    return new WindowStatusResult(false); // Window is closed
                }
            }

            Console.WriteLine("Received unexpected status: " + parts[4]);
            return new WindowStatusResult(null, $"Invalid status value received: {parts[4]}.");
        }

        public async Task SetWindowAngle(int GreenHouseId, int angle)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,{angle}";
            await clientHandler.SendMessageAsync(message);
        }
    }
}
