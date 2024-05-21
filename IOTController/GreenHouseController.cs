using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOTController.Controllers
{
    [ApiController]
    [Route("{GreenHouseId}")]
    public class GreenhouseController : ControllerBase
    {
        private readonly GreenhouseService _greenhouseService;

        public GreenhouseController(GreenhouseService greenhouseService)
        {
            _greenhouseService = greenhouseService;
        }

        [HttpPost("openWindow")]
        public async Task<IActionResult> OpenWindow(int GreenHouseId)
        {
            await _greenhouseService.OpenWindow(GreenHouseId);
            return NoContent(); // 204 No Content
        }

        [HttpPost("closeWindow")]
        public async Task<IActionResult> CloseWindow(int GreenHouseId)
        {
            await _greenhouseService.CloseWindow(GreenHouseId);
            return NoContent(); // 204 No Content
        }

        [HttpPost("getStatus")]
        public async Task<IActionResult> GetStatus(int GreenHouseId)
        {
            await _greenhouseService.GetWindowStatus(GreenHouseId);
            return NoContent(); // 204 No Content
        }
        
    }
}