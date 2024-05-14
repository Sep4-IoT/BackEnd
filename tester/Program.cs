using Application.Logic;

public class Program
{
    public static async Task Main()
    {
        ArduinoLogic arduinoLogic = new ArduinoLogic();
        arduinoLogic.OpenWindow(2);
    }
}