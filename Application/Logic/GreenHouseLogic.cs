using Application.DAOInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Model;

namespace Application.Logic;

public class GreenHouseLogic : IGreenHouseLogic
{
    private readonly IGreenHouseDAO greenHouseDao;
    private IGreenHouseLogic _icontroller;

    public GreenHouseLogic(IGreenHouseDAO greenHouseDao)
    {
        this.greenHouseDao = greenHouseDao;
        _icontroller = new GreenHouseLogic(greenHouseDao);
    }
    
    public Task<IEnumerable<GreenHouse>> GetAsync(SearchGreenHouseDTO searchParameters)
    {
        return greenHouseDao.GetAsync(searchParameters);
    }

    public async Task<GreenHouse> CreateAsync(GreenHouseCreationDTO dto)
    {
        GreenHouse? existing = await greenHouseDao.GetByNameAsync(dto.GreenHouseName);
        if (existing != null)
            throw new Exception("GreenHouse Name already exists!");

        ValidateData(dto);
        GreenHouse toCreate = new GreenHouse(
            dto.GreenHouseName,
            dto.Description,
            dto.Temperature,
            dto.LightIntensity,
            dto.Co2Levels,
            dto.Humidity,
            false);
            
        
        GreenHouse created = await greenHouseDao.CreateAsync(toCreate);
    
        return created;
    }
    

    //NOT WORKING
    public async Task UpdateAsync(int greenHouseId, UpdateGreenHouseDTO dto)
    {
        GreenHouse? existing = await greenHouseDao.GetByIdAsync(greenHouseId);
        
        if (existing == null)
        {
            throw new Exception($"Greenhouse with ID {greenHouseId} not found!");
        }

        bool windowStateChanged = dto.IsWindowOpen != existing.IsWindowOpen;

        GreenHouse updated = new GreenHouse(
            greenHouseId,
            dto.GreenHouseName,
            dto.Description,
            dto.Temperature,
            dto.LightIntensity,
            dto.Co2Levels,
            dto.Humidity,
            dto.IsWindowOpen);
        
            //_icontroller.ChangeWindowStatus(greenHouseId, updated.IsWindowOpen.GetValueOrDefault());

        
            await greenHouseDao.UpdateAsync(updated);
        
    }


    private static void ValidateData(GreenHouseCreationDTO greenHouseCreation)
    {
        string userName = greenHouseCreation.GreenHouseName;

        if (userName.Length < 3)
            throw new Exception("GreenHouse name must be at least 3 characters!");

        if (userName.Length > 20)
            throw new Exception("GreenHouse must be less than 21 characters!");
    }
    
    
    
}