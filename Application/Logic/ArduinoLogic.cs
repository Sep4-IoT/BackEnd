using Application.LogicInterfaces;
using TCPController;
using System.Threading.Tasks;

namespace Application.Logic
{
    public class ArduinoLogic : IArduinoLogic
    {
        private Controller controller;
        
        public ArduinoLogic()
        {
            controller = new Controller();
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
            return await controller.GetWindowStatus(GreenHouseId);
        }
    }
}