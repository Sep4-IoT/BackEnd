using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IOTController;

[Route("api/[controller]")]
[ApiController]
public class GreenHouseController : ControllerBase
{
    private readonly GreenHouseService _greenHouseService;
    private readonly ClientHandler _clientHandler;

    public GreenHouseController(GreenHouseService greenHouseService, ClientHandler clientHandler)
    {
        _greenHouseService = greenHouseService;
        _clientHandler = clientHandler;
        _greenHouseService.Initialize(_clientHandler);
    }

    [HttpGet("{id}/temperature")]
    public async Task<IActionResult> GetTemperature(int id)
    {
        var result = await _greenHouseService.GetTemperature(id);
        if (result.Temperature == null)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Temperature);
    }

    [HttpGet("{id}/windowstatus")]
    public async Task<IActionResult> GetWindowStatus(int id)
    {
        var result = await _greenHouseService.GetWindowStatus(id);
        if (result.IsWindowOpen == null)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.IsWindowOpen);
    }

    [HttpPost("{id}/openwindow")]
    public async Task<IActionResult> OpenWindow(int id)
    {
        var response = await _greenHouseService.OpenWindow(id);
        return Ok(response);
    }

    [HttpPost("{id}/closewindow")]
    public async Task<IActionResult> CloseWindow(int id)
    {
        var response = await _greenHouseService.CloseWindow(id);
        return Ok(response);
    }
}