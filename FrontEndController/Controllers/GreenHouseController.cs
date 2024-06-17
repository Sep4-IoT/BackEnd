using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTOs;

namespace BackEnd.Controllers
{
    [Authorize]
    [ApiController]
    [Route("SEP4")]
    public class GreenHouseController : ControllerBase
    {
        private readonly HttpClient _dbApiClient;
        private readonly HttpClient _iotControllerClient;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        private const string HardcodedUsername = "admin";
        private const string HardcodedPassword = "via";

        public GreenHouseController(IHttpClientFactory httpClientFactory, IConfiguration configuration,
            TokenService tokenService)
        {
            _dbApiClient = httpClientFactory.CreateClient("DBAPI");
            _iotControllerClient = httpClientFactory.CreateClient("IOTController");
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            if (userLogin.Username == HardcodedUsername && userLogin.Password == HardcodedPassword)
            {
                var token = _tokenService.GenerateToken(userLogin.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        

        [Authorize]
        [HttpGet("greenhouses/{greenHouseId}/current")]
        public async Task<ActionResult<GreenHouse>> GetByIdAsync(int greenHouseId)
        {
            try
            {
                var requestUri = $"/GreenHouse/{greenHouseId}";
                var response = await _dbApiClient.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();
                var greenHouseDTO = await response.Content.ReadFromJsonAsync<GreenHouseDTO>();
                return Ok(greenHouseDTO);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPatch("greenhouses/{greenHouseId}")]
        public async Task<ActionResult> UpdateAsync(int greenHouseId, [FromBody] GreenHouseUpdateDTO updateDto)
        {
            try
            {
                if (updateDto.IsWindowOpen.HasValue)
                {
                    var actionUri = updateDto.IsWindowOpen.Value
                        ? $"Greenhouse/{greenHouseId}/openWindow"
                        : $"Greenhouse/{greenHouseId}/closeWindow";

                    await _iotControllerClient.PostAsync(actionUri, null);
                }

                if (!string.IsNullOrEmpty(updateDto.Name))
                {
                    var nameUri = $"Greenhouse/{greenHouseId}/changeName/{updateDto.Name}";
                    await _dbApiClient.PostAsync(nameUri, null);
                }

                if (!string.IsNullOrEmpty(updateDto.Description))
                {
                    var descriptionUri = $"Greenhouse/{greenHouseId}/changeDescription/{updateDto.Description}";
                    await _dbApiClient.PostAsync(descriptionUri, null);
                }

                return Accepted();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }
        }


        [Authorize]
        [HttpGet("greenhouses/{greenHouseId}/history")]
        public async Task<ActionResult<List<GreenHouse>>> GetAllAsync(string greenHouseId)
        {
            try
            {
                var requestUri = $"/GreenHouse/getAll/{greenHouseId}";
                var response = await _dbApiClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var greenHouseDateList = await response.Content.ReadFromJsonAsync<GreenHouseDateList>();

                List <GreenHouse> greenHouseList = new List<GreenHouse>();
                foreach (var greenHouse in greenHouseDateList.GreenHouses)
                {
                    greenHouseList.Add(greenHouse);
                }
                return Ok(greenHouseList);
            }
            catch (Exception e)
            {
                // Log the exception and return a 500 status code
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }
        }
        
        
    }
}
