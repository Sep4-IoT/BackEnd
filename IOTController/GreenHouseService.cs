using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IOTController;

public class GreenHouseService
{
    private readonly GreenHouseContext _context;
    private ClientHandler _clientHandler;

    public GreenHouseService(GreenHouseContext context)
    {
        _context = context;
    }
    
    public void Initialize(ClientHandler clientHandler)
    {
        _clientHandler = clientHandler;
    }

    public async Task<string> OpenWindow(int greenHouseId)
    {
        string message = $"REQ,{greenHouseId},SET,SER,180";
        await _clientHandler.SendMessageAsync(message);
        return await _clientHandler.ReceiveMessageAsync();
    }

    public async Task<string> CloseWindow(int greenHouseId)
    {
        string message = $"REQ,{greenHouseId},SET,SER,0";
        await _clientHandler.SendMessageAsync(message);
        return await _clientHandler.ReceiveMessageAsync();
    }

    public async Task<WindowStatusResult> GetWindowStatus(int greenHouseId)
    {
        string message = $"REQ,{greenHouseId},GET,SER";
        await _clientHandler.SendMessageAsync(message);

        string response = await _clientHandler.ReceiveMessageAsync();
        Console.WriteLine("Received: " + response);

        string[] parts = response.Split(',');
        if (parts.Length < 4)
        {
            Console.WriteLine("Received malformed response: " + response);
            return new WindowStatusResult(null, "Received malformed response.");
        }

        if (parts[0] == "ACK" && parts[1] == greenHouseId.ToString() && parts[2] == "GET" && parts[3] == "SER")
        {
            if (parts[4] == "180")
            {
                return new WindowStatusResult(true);
            }
            if (parts[4] == "0")
            {
                return new WindowStatusResult(false);
            }
        }

        Console.WriteLine("Received unexpected status: " + parts[4]);
        return new WindowStatusResult(null, $"Invalid status value received: {parts[4]}.");
    }

    public async Task<TemperatureResult> GetTemperature(int greenHouseId)
    {
        EnsureClientHandlerIsInitialized();
        string message = $"REQ,{greenHouseId},GET,TEM";
        await _clientHandler.SendMessageAsync(message);

        string response = await _clientHandler.ReceiveMessageAsync();
        Console.WriteLine("Received: " + response);

        string[] parts = response.Split(',');
        if (parts.Length < 4)
        {
            Console.WriteLine("Received malformed response: " + response);
            return new TemperatureResult(null, "Received malformed response.");
        }

        if (double.TryParse(parts[4], out double temperature))
        {
            try
            {
                var greenHouse = await _context.GreenHouses.FindAsync(greenHouseId);
                if (greenHouse == null)
                {
                    throw new Exception($"GreenHouse with ID {greenHouseId} does not exist!");
                }

                greenHouse.Temperature = temperature;
                _context.GreenHouses.Update(greenHouse);
                await _context.SaveChangesAsync();

                Console.WriteLine("Temperature updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating temperature: " + ex.Message);
                return new TemperatureResult(null, "Error updating temperature: " + ex.Message);
            }
            return new TemperatureResult(temperature);
        }
        else
        {
            Console.WriteLine("Received unexpected status: " + parts[4]);
            return new TemperatureResult(null, $"Invalid status value received: {parts[4]}.");
        }
    }

    public async Task SetWindowAngle(int greenHouseId, int angle)
    {
        string message = $"REQ,{greenHouseId},SET,SER,{angle}";
        await _clientHandler.SendMessageAsync(message);
    }
    
    private void EnsureClientHandlerIsInitialized()
    {
        if (_clientHandler == null)
        {
            throw new InvalidOperationException("ClientHandler is not initialized.");
        }
    }
}

public class WindowStatusResult
{
    public bool? IsWindowOpen { get; set; }
    public string ErrorMessage { get; set; }

    public WindowStatusResult(bool? isWindowOpen, string errorMessage = null)
    {
        IsWindowOpen = isWindowOpen;
        ErrorMessage = errorMessage;
    }
}

public class TemperatureResult
{
    public double? Temperature { get; set; }
    public string ErrorMessage { get; set; }

    public TemperatureResult(double? temperature, string errorMessage = null)
    {
        Temperature = temperature;
        ErrorMessage = errorMessage;
    }
}
