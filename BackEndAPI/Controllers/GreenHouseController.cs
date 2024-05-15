using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreenHouseController : ControllerBase
    {
        private readonly IGreenHouseLogic greenHouseLogic;
        private readonly HttpClient httpClient;

        public GreenHouseController(IGreenHouseLogic greenHouseLogic)
        {
            this.greenHouseLogic = greenHouseLogic;
            httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:6000/") };
        }

        [HttpPost]
        public async Task<ActionResult<GreenHouse>> CreateAsync([FromBody] GreenHouseCreationDTO dto)
        {
            try
            {
                GreenHouse greenHouse = await greenHouseLogic.CreateAsync(dto);
                return Created($"/GreenHouse/{greenHouse.GreenHouseName}", greenHouse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{greenHouseId}")]
        public async Task<ActionResult<GreenHouse>> GetAsync(int greenHouseId)
        {
            try
            {
                GreenHouse greenHouse = await greenHouseLogic.GetByIdAsync(greenHouseId);

                // Get the current status from the IOT server
                string iotStatus = await FetchIOTStatusFromServerAsync(greenHouseId);
                greenHouse.IsWindowOpen = iotStatus == "Open";
                string iotTemperature = await FetchIOTTemperatureFromServerAsync(greenHouseId);
                if (double.TryParse(iotTemperature, out double temperature))
                {
                    greenHouse.Temperature = temperature;
                }
                else
                {
                    Console.WriteLine("Error: Unable to parse temperature from server response.");
                }
                
                return Ok(greenHouse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<GreenHouse>>> GetByOwnerIdAsync(int ownerId)
        {
            try
            {
                IEnumerable<GreenHouse> greenHouses = await greenHouseLogic.GetByOwnerIdAsync(ownerId);
                return Ok(greenHouses);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch("{greenHouseId}")]
        public async Task<ActionResult<UpdateGreenHouseDTO>> UpdateAsync(int greenHouseId, string? greenHouseName, string? description, bool? isWindowOpen)
        {
            try
            {
                var existingGreenHouse = await greenHouseLogic.GetByIdAsync(greenHouseId);

                greenHouseName ??= existingGreenHouse.GreenHouseName;
                description ??= existingGreenHouse.Description;

                if (isWindowOpen.HasValue && isWindowOpen != existingGreenHouse.IsWindowOpen)
                {
                    if (isWindowOpen.Value)
                    {
                        await OpenWindowAsync(greenHouseId);
                    }
                    else
                    {
                        await CloseWindowAsync(greenHouseId);
                    }
                }
                else
                {
                    isWindowOpen = existingGreenHouse.IsWindowOpen;
                }

                UpdateGreenHouseDTO updated = new UpdateGreenHouseDTO(greenHouseId, greenHouseName, description, isWindowOpen);
                var greenHouse = await greenHouseLogic.UpdateAsync(updated);
                return Ok(greenHouse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, e.Message);
            }
        }
        
        
        private async Task<string> FetchIOTStatusFromServerAsync(int greenHouseId)
        {
            var response = await httpClient.GetAsync($"GreenHouse/IOT/{greenHouseId}/getStatus?clientId=1");
            response.EnsureSuccessStatusCode();
            string status = await response.Content.ReadAsStringAsync();
            return status;
        }
        
        private async Task<string> FetchIOTTemperatureFromServerAsync(int greenHouseId)
        {
            var response = await httpClient.GetAsync($"GreenHouse/IOT/{greenHouseId}/getTemperature?clientId=1");
            response.EnsureSuccessStatusCode();
            string status = await response.Content.ReadAsStringAsync();
            return status;
        }

        private async Task OpenWindowAsync(int greenHouseId)
        {
            var response = await httpClient.PostAsync($"GreenHouse/IOT/{greenHouseId}/openWindow?clientId=1", null);
            response.EnsureSuccessStatusCode();
        }

        private async Task CloseWindowAsync(int greenHouseId)
        {
            var response = await httpClient.PostAsync($"GreenHouse/IOT/{greenHouseId}/closeWindow?clientId=1", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
