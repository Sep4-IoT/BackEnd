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
            await _greenhouseManager.OpenWindow(GreenHouseId);
        }

        public async Task CloseWindow(int GreenHouseId)
        {
            await _greenhouseManager.CloseWindow(GreenHouseId);
        }
        
    }
}