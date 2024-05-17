using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class GreenHouseController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GreenHouseController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("DBAPI");
        _configuration = configuration;
    }
    
    [HttpGet("{greenHouseId}")]
    public async Task<ActionResult<GreenHouse>> GetByIdAsync(int greenHouseId)
    { 
        try
        {
            var requestUri = $"/api/GreenHouse/{greenHouseId}";
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
    
    [HttpPatch("{greenHouseId}")]
    public async Task<ActionResult> UpdateAsync(int greenHouseId,
        string? greenHouseName, string? description, bool? isWindowOpen)
    {
        try
        {
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}