using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOTController.Controllers
{
    [ApiController]
    [Route("IOT/{GreenHouseId}")]
    public class GreenhouseController : ControllerBase
    {
        private readonly GreenhouseService _greenhouseService;

        public GreenhouseController(GreenhouseService greenhouseService)
        {
            _greenhouseService = greenhouseService;
        }

        [HttpPatch("openWindow")]
        public async Task<IActionResult> OpenWindow(int GreenHouseId)
        {
            await _greenhouseService.OpenWindow(GreenHouseId);
            return Ok();
        }

        [HttpPatch("closeWindow")]
        public async Task<IActionResult> CloseWindow(int GreenHouseId)
        {
            await _greenhouseService.CloseWindow(GreenHouseId);
            return Ok();
        }

        [HttpGet("getStatus")]
        public async Task<IActionResult> GetStatus(int GreenHouseId)
        {
            var statusResult = await _greenhouseService.GetWindowStatus(GreenHouseId);
            if (statusResult.ErrorMessage != null)
            {
                return BadRequest(statusResult.ErrorMessage);
            }

            string statusDescription = statusResult.IsWindowOpen.HasValue
                ? (statusResult.IsWindowOpen.Value ? "Open" : "Closed")
                : "Status not determined";

            return Ok(statusDescription);
        }
    }
}