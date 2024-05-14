using Application.LogicInterfaces;
using System.Threading.Tasks;
using IOTController;

namespace Application.Logic
{
    public class ArduinoLogic : IArduinoLogic
    {
        private Controller controller;
        
        public ArduinoLogic()
        {
            // Access the singleton instance of Controller
            controller = Controller.Instance;
        }

        public async Task OpenWindow(int GreenHouseId)
        {
            await controller.OpenWindow(GreenHouseId);
        }

        public async Task CloseWindow(int GreenHouseId)
        {
            await controller.CloseWindow(GreenHouseId);
        }
        
        public async Task<bool> GetWindowStatus(int GreenHouseId)
        {
            var statusResult = await controller.GetWindowStatus(GreenHouseId);
            if (statusResult.ErrorMessage != null)
            {
                // Assuming you log the error or handle it as needed
                Console.WriteLine($"Error retrieving window status for Greenhouse {GreenHouseId}: {statusResult.ErrorMessage}");
                return false;  // or handle the error differently
            }
            return statusResult.IsWindowOpen ?? false; // Convert nullable bool to bool by assuming false if null
        }
        
    }
}