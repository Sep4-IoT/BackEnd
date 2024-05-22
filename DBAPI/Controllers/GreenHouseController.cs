using Domain.Model;
using WebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GreenHouseController : ControllerBase
    {
        private readonly GreenHouseRepository _repository;

        public GreenHouseController(GreenHouseRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var greenHouses = await _repository.GetAllAsync();
            return Ok(greenHouses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var greenHouse = await _repository.GetByIdAsync(id);
            if (greenHouse == null) return NotFound();
            return Ok(greenHouse);
        }

        [HttpPatch("{id}/openWindow")]  // Use HttpPatch for partial updates
        public async Task<IActionResult> OpenWindow(string id)
        {
            var existingGreenHouse = await _repository.GetByIdAsync(id);
            if (existingGreenHouse == null) return NotFound();

            existingGreenHouse.IsWindowOpen = true;
            await _repository.UpdateAsync(existingGreenHouse);
            return Ok();
        }

        [HttpPatch("{id}/closeWindow")]  // Use HttpPatch for partial updates
        public async Task<IActionResult> CloseWindow(string id)
        {
            var existingGreenHouse = await _repository.GetByIdAsync(id);
            if (existingGreenHouse == null) return NotFound();

            existingGreenHouse.IsWindowOpen = false;
            await _repository.UpdateAsync(existingGreenHouse);
            return Ok();
        }
    }
}