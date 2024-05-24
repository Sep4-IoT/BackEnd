using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOTController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreenhouseController : ControllerBase
    {
        private readonly GreenhouseService _greenhouseService;

        public GreenhouseController(GreenhouseService greenhouseService)
        {
            _greenhouseService = greenhouseService;
        }

        [HttpPost("{greenhouseId}/openWindow")]
        public async Task<IActionResult> OpenWindow(int greenhouseId)
        {
            await _greenhouseService.OpenWindow(greenhouseId);
            return NoContent(); // 204 No Content
        }

        [HttpPost("{greenhouseId}/closeWindow")]
        public async Task<IActionResult> CloseWindow(int greenhouseId)
        {
            await _greenhouseService.CloseWindow(greenhouseId);
            return NoContent(); // 204 No Content
        }
    }
}