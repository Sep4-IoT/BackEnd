using Application.LogicInterfaces;

namespace IOTController;

public class GreenhouseService
{
        private readonly ClientHandler clientHandler;
        private GreenHouseManager _greenhouseManager;
        private readonly IGreenHouseLogic greenHouseLogic;
        
        public void Initialize(ClientHandler newClientHandler)
        {
            if (_greenhouseManager == null)
            {
                _greenhouseManager = new GreenHouseManager(newClientHandler, greenHouseLogic);
            }
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

        public async Task<GreenHouseManager.WindowStatusResult> GetWindowStatus(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await _greenhouseManager.GetWindowStatus(GreenHouseId);
        }
        
        public async Task<GreenHouseManager.TemperatureResult> GetTemperature(int GreenHouseId)
        {
            if (_greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await _greenhouseManager.GetTemperature(GreenHouseId);
        }
        
        
}