namespace Application.LogicInterfaces
{
    // Defines the logic layer interface for Arduino-related operations
    public interface IArduinoLogic
    {
        // Task to open a window in a greenhouse
        Task OpenWindow(int GreenHouseId);

        // Task to close a window in a greenhouse
        Task CloseWindow(int GreenHouseId);

        // Task to get the status of a window in a greenhouse, returning true if open
        Task<bool> GetWindowStatus(int GreenHouseId);
    }
}