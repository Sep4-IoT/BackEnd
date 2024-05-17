
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class GreenHouseController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GreenHouseController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    [HttpGet("{greenHouseId}")]
    public async Task<ActionResult<GreenHouse>> GetByIdAsync(int greenHouseId)
    { 
        try
        {
            var requestUri = $"{_configuration["http://localhost:5000"]}/api/GreenHouse/{greenHouseId}";
            var response = await _httpClient.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();
            var greenHouse = await response.Content.ReadFromJsonAsync<GreenHouse>();

            return Ok(greenHouse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
