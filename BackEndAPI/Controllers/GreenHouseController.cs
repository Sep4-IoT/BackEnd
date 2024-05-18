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
            var requestUri = $"/api/GreenHouse/{greenHouseId}";
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
    public async Task<ActionResult> UpdateAsync(int greenHouseId,
        string? greenHouseName, string? description, bool? isWindowOpen)
    {
        try
        {
            // Fetch the current window status from DBAPI
            var getRequestUri = $"/api/GreenHouse/{greenHouseId}";
            var getResponse = await _dbApiClient.GetAsync(getRequestUri);
            getResponse.EnsureSuccessStatusCode();
            var greenHouse = await getResponse.Content.ReadFromJsonAsync<GreenHouse>();

            if (greenHouse == null)
            {
                return NotFound("Greenhouse not found");
            }

            // Determine the current status and desired status
            var currentStatus = greenHouse.IsWindowOpen.HasValue && greenHouse.IsWindowOpen.Value;
            if (isWindowOpen.HasValue && isWindowOpen.Value != currentStatus)
            {
                var actionUri = isWindowOpen.Value
                    ? $"/IOT/{greenHouseId}/openWindow"
                    : $"/IOT/{greenHouseId}/closeWindow";

                try
                {
                    // Send the request to IOTController
                    var postResponse = await _iotControllerClient.PostAsync(actionUri, null);
                    postResponse.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Handle 404 error from IOTController (client not connected)
                    Console.WriteLine($"IOTController returned 404 for {actionUri}: {ex.Message}");
                    // Optionally, log the 404 error or take other actions
                }
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
