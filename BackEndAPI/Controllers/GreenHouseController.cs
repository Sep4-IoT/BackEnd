using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class  GreenHouseController : ControllerBase
{
    private readonly IGreenHouseLogic greenHouseLogic;

    public GreenHouseController(IGreenHouseLogic greenHouseLogic)
    {
        this.greenHouseLogic = greenHouseLogic;
    }
    
    [HttpPost]
    public async Task<ActionResult<GreenHouse>> CreateAsync(GreenHouseCreationDTO dto)
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
    
    [HttpGet ]
    public async Task<ActionResult<IEnumerable<GreenHouse>>> GetAsync([FromQuery] int? greenHouseId)
    { 
        try
        {
            SearchGreenHouseDTO parameters = new(greenHouseId);
            IEnumerable<GreenHouse> users = await greenHouseLogic.GetAsync(parameters);
            return Ok(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpPatch("{greenHouseId}")]
    public async Task<ActionResult<GreenHouse>> UpdateAsync(int greenHouseId, [FromBody] UpdateGreenHouseDTO dto)
    {
        try
        {
            Console.WriteLine(greenHouseId);
            await greenHouseLogic.UpdateAsync(greenHouseId, dto);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }


    
}