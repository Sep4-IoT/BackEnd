using System;
using System.Threading.Tasks;

namespace IOTController
{
    public class GreenhouseManager
    {
        private Server server;

        public GreenhouseManager(Server server)
        {
            this.server = server;
        }

        public async Task OpenWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,180";
            await server.BroadcastMessageAsync(message);
        }

        public async Task CloseWindow(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},SET,SER,0";
            await server.BroadcastMessageAsync(message);
        }

        public async Task GetWindowStatus(int GreenHouseId)
        {
            string message = $"REQ,{GreenHouseId},GET,SER";
            await server.BroadcastMessageAsync(message);
        }
    }
}