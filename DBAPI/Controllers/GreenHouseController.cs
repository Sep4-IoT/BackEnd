using Domain.Model;
using WebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GreenHouseController : ControllerBase
    {
        private readonly GreenHouseDateListRepository _repository;

        public GreenHouseController(GreenHouseDateListRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var greenHouseDateList = await _repository.GetByIdAsync(id);
            if (greenHouseDateList == null) return NotFound();

            var latestGreenHouse = greenHouseDateList.GreenHouses.OrderByDescending(gh => gh.Date).FirstOrDefault();
            if (latestGreenHouse == null) return NotFound();

            var dto = new GreenHouseDTO
            {
                GreenHouseId = latestGreenHouse.GreenHouseId,
                GreenHouseName = latestGreenHouse.GreenHouseName,
                Description = latestGreenHouse.Description,
                Temperature = latestGreenHouse.Temperature,
                LightIntensity = latestGreenHouse.LightIntensity,
                Co2Levels = latestGreenHouse.Co2Levels,
                Humidity = latestGreenHouse.Humidity,
                IsWindowOpen = latestGreenHouse.IsWindowOpen
            };

            return Ok(dto);
        }

        [HttpGet("getAll/{id}")]
        public async Task<IActionResult> GetAll(string id)
        {
            var greenHouseDateList = await _repository.GetByIdAsync(id);
            if (greenHouseDateList == null) return NotFound();
            return Ok(greenHouseDateList);
        }

        // Open window on newest greenhouse data
        [HttpPost("{id}/openWindow")]
        public async Task<IActionResult> OpenWindow(string id)
        {
            var greenHouseDateList = await _repository.GetByIdAsync(id);
            if (greenHouseDateList == null) return NotFound();

            var latestGreenHouse = greenHouseDateList.GreenHouses.OrderByDescending(gh => gh.Date).FirstOrDefault();
            if (latestGreenHouse == null) return NotFound();

            latestGreenHouse.IsWindowOpen = true;
            await _repository.UpdateAsync(greenHouseDateList);

            return Ok();
        }

        // Close window on newest greenhouse data
        [HttpPost("{id}/closeWindow")]
        public async Task<IActionResult> CloseWindow(string id)
        {
            var greenHouseDateList = await _repository.GetByIdAsync(id);
            if (greenHouseDateList == null) return NotFound();

            var latestGreenHouse = greenHouseDateList.GreenHouses.OrderByDescending(gh => gh.Date).FirstOrDefault();
            if (latestGreenHouse == null) return NotFound();

            latestGreenHouse.IsWindowOpen = false;
            await _repository.UpdateAsync(greenHouseDateList);

            return Ok();
        }

        // Update temperature on newest greenhouse data
        [HttpPost("{id}/updateTemperature/{temperature}")]
        public async Task<IActionResult> UpdateTemperature(string id, double temperature)
        {
            var greenHouseDateList = await _repository.GetByIdAsync(id);
            if (greenHouseDateList == null) return NotFound();

            var latestGreenHouse = greenHouseDateList.GreenHouses.OrderByDescending(gh => gh.Date).FirstOrDefault();
            if (latestGreenHouse == null) return NotFound();

            latestGreenHouse.Temperature = temperature;
            await _repository.UpdateAsync(greenHouseDateList);

            return Ok();
        }

        // Update humidity on newest greenhouse data
        [HttpPost("{id}/updateHumidity/{humidity}")]
        public async Task<IActionResult> UpdateHumidity(string id, double humidity)
        {
            var greenHouseDateList = await _repository.GetByIdAsync(id);
            if (greenHouseDateList == null) return NotFound();

            var latestGreenHouse = greenHouseDateList.GreenHouses.OrderByDescending(gh => gh.Date).FirstOrDefault();
            if (latestGreenHouse == null) return NotFound();

            latestGreenHouse.Humidity = humidity;
            await _repository.UpdateAsync(greenHouseDateList);

            return Ok();
        }

        // Update light intensity on newest greenhouse data
        [HttpPost("{id}/updateLight/{lightIntensity}")]
        public async Task<IActionResult> UpdateLight(string id, double lightIntensity)
        {
            var greenHouseDateList = await _repository.GetByIdAsync(id);
            if (greenHouseDateList == null) return NotFound();

            var latestGreenHouse = greenHouseDateList.GreenHouses.OrderByDescending(gh => gh.Date).FirstOrDefault();
            if (latestGreenHouse == null) return NotFound();

            latestGreenHouse.LightIntensity = lightIntensity;
            await _repository.UpdateAsync(greenHouseDateList);

            return Ok();
        }
    }
}
