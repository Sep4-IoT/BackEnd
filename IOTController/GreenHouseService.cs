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

        public Task OpenWindow(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");

            return _greenhouseManager.OpenWindow(GreenHouseId);
        }

        public Task CloseWindow(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");

            return _greenhouseManager.CloseWindow(GreenHouseId);
        }

        public Task<GreenhouseManager.WindowStatusResult> GetWindowStatus(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new System.InvalidOperationException("GreenhouseManager is not initialized.");

            return _greenhouseManager.GetWindowStatus(GreenHouseId);
        }
    }
}