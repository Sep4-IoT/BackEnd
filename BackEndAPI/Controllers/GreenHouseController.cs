using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Domain.DTOs;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class GreenHouseController : ControllerBase
{
    private readonly HttpClient _dbApiClient;
    private readonly HttpClient _iotControllerClient;
    private readonly IConfiguration _configuration;

    public GreenHouseController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _dbApiClient = httpClientFactory.CreateClient("DBAPI");
        _iotControllerClient = httpClientFactory.CreateClient("IOTController");
        _configuration = configuration;
    }
    
    [HttpGet("{greenHouseId}")]
    public async Task<ActionResult<GreenHouse>> GetByIdAsync(int greenHouseId)
    { 
        try
        {
            var requestUri = $"/GreenHouse/{greenHouseId}";
            var response = await _dbApiClient.GetAsync(requestUri);

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
    public async Task<ActionResult> UpdateAsync(int greenHouseId, [FromBody] GreenHouseUpdateDTO updateDto)
    {
        try
        {
            Console.WriteLine("Begin updating GreenHouse");
            if (updateDto.IsWindowOpen.HasValue)
            {
                var actionUri = updateDto.IsWindowOpen.Value
                    ? $"{greenHouseId}/openWindow"
                    : $"{greenHouseId}/closeWindow";
                
                await _iotControllerClient.PostAsync(actionUri, null);
            }

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
