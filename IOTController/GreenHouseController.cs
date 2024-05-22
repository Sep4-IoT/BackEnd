using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOTController.Controllers
{
    [ApiController]
<<<<<<< Updated upstream:IOTController/GreenHouseController.cs
    [Route("{GreenHouseId}")]
=======
    [Route("[controller]")]
>>>>>>> Stashed changes:IOTController/Controllers/GreenHouseController.cs
    public class GreenhouseController : ControllerBase
    {
        private readonly GreenhouseService _greenhouseService;

        public GreenhouseController(GreenhouseService greenhouseService)
        {
            _greenhouseService = greenhouseService;
        }

<<<<<<< Updated upstream:IOTController/GreenHouseController.cs
        [HttpPost("openWindow")]
        public async Task<IActionResult> OpenWindow(int GreenHouseId)
=======
        [HttpPost("{greenhouseId}/openWindow")]
        public async Task<IActionResult> OpenWindow(int greenhouseId)
>>>>>>> Stashed changes:IOTController/Controllers/GreenHouseController.cs
        {
            await _greenhouseService.OpenWindow(GreenHouseId);
            return NoContent(); // 204 No Content
        }

<<<<<<< Updated upstream:IOTController/GreenHouseController.cs
        [HttpPost("closeWindow")]
        public async Task<IActionResult> CloseWindow(int GreenHouseId)
=======
        [HttpPost("{greenhouseId}/closeWindow")]
        public async Task<IActionResult> CloseWindow(int greenhouseId)
>>>>>>> Stashed changes:IOTController/Controllers/GreenHouseController.cs
        {
            await _greenhouseService.CloseWindow(GreenHouseId);
            return NoContent(); // 204 No Content
        }

<<<<<<< Updated upstream:IOTController/GreenHouseController.cs
        [HttpPost("getStatus")]
        public async Task<IActionResult> GetStatus(int GreenHouseId)
=======
        [HttpPost("{greenhouseId}/getWindowStatus")]
        public async Task<IActionResult> GetStatus(int greenhouseId)
>>>>>>> Stashed changes:IOTController/Controllers/GreenHouseController.cs
        {
            await _greenhouseService.GetWindowStatus(GreenHouseId);
            return NoContent(); // 204 No Content
        }
        
    }
}