using System.Threading.Tasks;

namespace IOTController
{
    public class GreenhouseService
    {
        private ClientHandler _clientHandler;
        private GreenhouseManager _greenhouseManager;

        public void Initialize(ClientHandler clientHandler)
        {
            _clientHandler = clientHandler;
            _greenhouseManager = new GreenhouseManager(clientHandler);
        }

        public async Task<string> OpenWindow(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await _greenhouseManager.OpenWindow(GreenHouseId);
        }

        public async Task<string> CloseWindow(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await _greenhouseManager.CloseWindow(GreenHouseId);
        }

        public async Task<GreenhouseManager.WindowStatusResult> GetWindowStatus(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await _greenhouseManager.GetWindowStatus(GreenHouseId);
        }
        
        public async Task<GreenhouseManager.TemperatureResult> GetTemperature(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await _greenhouseManager.GetTemperature(GreenHouseId);
        }
        
        
    }
}