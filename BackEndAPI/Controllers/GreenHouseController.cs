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
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GreenHouse>>> GetAsync([FromQuery] int? greenHouseId)
    { 
        try
        {
            SearchGreenHouseDTO parameters = new(greenHouseId);
            IEnumerable<GreenHouse> greenHouses = await greenHouseLogic.GetAsync(parameters);
            return Ok(greenHouses);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet("{ownerId}")]
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
    public async Task<ActionResult<UpdateGreenHouseDTO>> UpdateAsync(int greenHouseId,
        string? greenHouseName, string? description, bool? isWindowOpen)
    {
        try
        {
            var existingGreenHouse = await greenHouseLogic.GetByIdAsync(greenHouseId);
            
            greenHouseName ??= existingGreenHouse.GreenHouseName;
            description ??= existingGreenHouse.Description;
            isWindowOpen ??= existingGreenHouse.IsWindowOpen;

            UpdateGreenHouseDTO updated =
                new UpdateGreenHouseDTO(greenHouseId, greenHouseName, description, isWindowOpen);
        
            var greenHouse = await greenHouseLogic.UpdateAsync(updated);
            return Ok(greenHouse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet("{greenHouseId}")]
    public async Task<ActionResult<IEnumerable<GreenHouse>>> GetByIdAsync(int greenHouseId)
    { 
        try
        {
            GreenHouse? greenHouse = await greenHouseLogic.GetByIdAsync(greenHouseId);
            return Ok(greenHouse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }



    
}