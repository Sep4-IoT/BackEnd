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

<<<<<<< Updated upstream
        /*[HttpPatch("{id}/window/{status}")]
        public async Task<IActionResult> UpdateWindowStatus(string id, bool status)
        {
            var existingGreenHouse = await _repository.GetByIdAsync(id);
            if (existingGreenHouse == null) return NotFound();

            existingGreenHouse.IsWindowOpen = status;
            await _repository.UpdateAsync(existingGreenHouse);
            //return 200 ok
            return Ok();
        }*/
        
        [HttpPost("{id}/openWindow")]
        public async Task<IActionResult> OpenWindow(string id)
=======
        [HttpPatch("{id}/openWindow")]
        public async Task<IActionResult> openWindow(string id)
>>>>>>> Stashed changes
        {
            var existingGreenHouse = await _repository.GetByIdAsync(id);
            if (existingGreenHouse == null) return NotFound();

            existingGreenHouse.IsWindowOpen = true;
            await _repository.UpdateAsync(existingGreenHouse);
            //return 200 ok
            return Ok();
        }
        
<<<<<<< Updated upstream
        [HttpPost("{id}/closeWindow")]
        public async Task<IActionResult> CloseWindow(string id)
=======
        [HttpPatch("{id}/closeWindow")]
        public async Task<IActionResult> closeWindow(string id)
>>>>>>> Stashed changes
        {
            var existingGreenHouse = await _repository.GetByIdAsync(id);
            if (existingGreenHouse == null) return NotFound();

            existingGreenHouse.IsWindowOpen = false;
            await _repository.UpdateAsync(existingGreenHouse);
            //return 200 ok
            return Ok();
        }
    }
}
