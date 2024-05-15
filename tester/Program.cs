using Application.Logic;
using IOTController;

public class Program
{
    public static async Task Main()
    {
        Controller controller = Controller.Instance;
        controller.getClient();
    }
}