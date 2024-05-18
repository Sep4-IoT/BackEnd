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

        public bool IsInitialized => _greenhouseManager != null;

        public async Task<string> OpenWindow(int GreenHouseId)
        {
            EnsureInitialized();
            return await _greenhouseManager.OpenWindow(GreenHouseId);
        }

        public async Task<string> CloseWindow(int GreenHouseId)
        {
            EnsureInitialized();
            return await _greenhouseManager.CloseWindow(GreenHouseId);
        }

        public async Task<GreenhouseManager.WindowStatusResult> GetWindowStatus(int GreenHouseId)
        {
            EnsureInitialized();
            return await _greenhouseManager.GetWindowStatus(GreenHouseId);
        }

        public async Task SetWindowAngle(int GreenHouseId, int angle)
        {
            EnsureInitialized();
            await _greenhouseManager.SetWindowAngle(GreenHouseId, angle);
        }

        private void EnsureInitialized()
        {
            if (_greenhouseManager == null)
            {
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");
            }
        }
    }
}