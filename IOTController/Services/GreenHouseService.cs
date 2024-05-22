using System.Threading.Tasks;

namespace IOTController
{
    public class GreenhouseService
    {
        private readonly GreenhouseManager _greenhouseManager;

        public GreenhouseService(GreenhouseManager greenhouseManager)
        {
            _greenhouseManager = greenhouseManager;
        }

        public async Task OpenWindow(int GreenHouseId)
        {
            await _greenhouseManager.OpenWindow(GreenHouseId);
        }

        public async Task CloseWindow(int GreenHouseId)
        {
            await _greenhouseManager.CloseWindow(GreenHouseId);
        }

        public async Task GetWindowStatus(int GreenHouseId)
        {
            await _greenhouseManager.GetWindowStatus(GreenHouseId);
        }
        
    }
}