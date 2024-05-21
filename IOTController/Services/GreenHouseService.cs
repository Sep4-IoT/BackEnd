using System.Threading.Tasks;

namespace IOTController
{
    public class GreenhouseService
    {
        private GreenhouseManager _greenhouseManager;

        public void Initialize(Server server)
        {
            _greenhouseManager = new GreenhouseManager(server);
        }

        public async Task OpenWindow(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");

            await _greenhouseManager.OpenWindow(GreenHouseId);
        }

        public async Task CloseWindow(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");

            await _greenhouseManager.CloseWindow(GreenHouseId);
        }

        public async Task GetWindowStatus(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");

            await _greenhouseManager.GetWindowStatus(GreenHouseId);
        }

        public async Task SetWindowAngle(int GreenHouseId, int angle)
        {
            if (_greenhouseManager == null)
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");

            await _greenhouseManager.SetWindowAngle(GreenHouseId, angle);
        }
    }
}