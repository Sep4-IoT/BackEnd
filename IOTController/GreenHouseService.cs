using Application.LogicInterfaces;

namespace IOTController
{
    public class GreenhouseService
    {
        private ClientHandler clientHandler;
        private GreenHouseManager greenhouseManager;
        private readonly IGreenHouseLogic greenHouseLogic;

        public GreenhouseService(IGreenHouseLogic greenHouseLogic)
        {
            this.greenHouseLogic = greenHouseLogic;
        }

        public void Initialize(ClientHandler newClientHandler)
        {
            clientHandler = newClientHandler;
            if (greenhouseManager == null)
            {
                greenhouseManager = new GreenHouseManager(clientHandler, greenHouseLogic);
            }
        }

        public async Task<string> OpenWindow(int GreenHouseId)
        {
            if (greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await greenhouseManager.OpenWindow(GreenHouseId);
        }

        public async Task<string> CloseWindow(int GreenHouseId)
        {
            if (greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await greenhouseManager.CloseWindow(GreenHouseId);
        }

        public async Task<GreenHouseManager.WindowStatusResult> GetWindowStatus(int GreenHouseId)
        {
            if (greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await greenhouseManager.GetWindowStatus(GreenHouseId);
        }

        public async Task<GreenHouseManager.TemperatureResult> GetTemperature(int GreenHouseId)
        {
            if (greenhouseManager == null)
                throw new InvalidOperationException("GreenhouseManager is not initialized.");

            return await greenhouseManager.GetTemperature(GreenHouseId);
        }
    }
}