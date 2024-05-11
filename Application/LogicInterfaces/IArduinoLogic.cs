using System.Threading.Tasks;

namespace Application.LogicInterfaces
{
    public interface IArduinoLogic
    {
        Task OpenWindow(int GreenHouseId);
        Task CloseWindow(int GreenHouseId);
        Task<bool> GetWindowStatus(int GreenHouseId);
    }
}