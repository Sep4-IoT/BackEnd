using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOTController.Controllers
{
    [ApiController]
    [Route("GreenHouse/IOT/{GreenHouseId}")]
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
            var response = await _greenhouseService.OpenWindow(GreenHouseId);
            if (response.StartsWith("ACK"))
            {
                return Ok(response);
            }
            return StatusCode(500, response);
        }

        [HttpPost("closeWindow")]
        public async Task<IActionResult> CloseWindow(int GreenHouseId)
        {
            var response = await _greenhouseService.CloseWindow(GreenHouseId);
            if (response.StartsWith("ACK"))
            {
                return Ok(response);
            }
            return StatusCode(500, response);
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
        
        [HttpGet("getTemperature")]
        public async Task<IActionResult> GetTemperature(int GreenHouseId)
        {
            var temperatureResult = await _greenhouseService.GetTemperature(GreenHouseId);
            if (temperatureResult.ErrorMessage != null)
            {
                return BadRequest(temperatureResult.ErrorMessage);
            }

            string temperatureDescription = temperatureResult.Temperature.HasValue ? temperatureResult.Temperature.Value.ToString() : "N/A";


            return Ok(temperatureDescription);
        }
    }
}