using Application.DAOInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Model;

namespace Application.Logic;

public class GreenHouseLogic : IGreenHouseLogic
{
    private readonly IGreenHouseDAO greenHouseDao;
    private readonly IUserDAO userDao;

    public GreenHouseLogic(IGreenHouseDAO greenHouseDao, IUserDAO userDao)
    {
        this.greenHouseDao = greenHouseDao;
        this.userDao = userDao;
    }
    
    public Task<IEnumerable<GreenHouse>> GetAsync(SearchGreenHouseDTO searchParameters)
    {
        return greenHouseDao.GetAsync(searchParameters);
    }

    public async Task<GreenHouse> CreateAsync(GreenHouseCreationDTO dto)
    {
        User? user = await userDao.GetByIdAsync(dto.OwnerId);
        if (user == null)
        {
            throw new Exception($"User with id {dto.OwnerId} was not found.");
        }
        
        GreenHouse? existing = await greenHouseDao.GetByNameAsync(dto.GreenHouseName);
        if (existing != null)
            throw new Exception("GreenHouse Name already exists!");

        ValidateData(dto);
        GreenHouse toCreate = new GreenHouse(
            dto.OwnerId,
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

    public async Task<GreenHouse> GetByIdAsync(int greenHouseId)
    {
        
        GreenHouse? greenHouse = await greenHouseDao.GetByIdAsync(greenHouseId);
        if (greenHouse == null)
        {
            throw new Exception($"Greenhouse with id {greenHouseId} not found");
        }

        return greenHouse;
    }


    public async Task<UpdateGreenHouseDTO> UpdateAsync(UpdateGreenHouseDTO updatedData)
    {
        UpdateGreenHouseDTO greenHouseDto = await greenHouseDao.UpdateAsync(updatedData);
        return greenHouseDto;
    }

    public Task<IEnumerable<GreenHouse>> GetByOwnerIdAsync(int ownerId)
    {
        return greenHouseDao.GetByOwnerIdAsync(ownerId);
    }

    public async Task UpdateTemperature(int greenhouseId, double temperature)
    {
        await greenHouseDao.UpdateTemperature(greenhouseId, temperature);
    }
    
    private static void ValidateData(GreenHouseCreationDTO greenHouseCreation)
    {
        string greenHouseName = greenHouseCreation.GreenHouseName;

        if (greenHouseName.Length < 3)
            throw new Exception("GreenHouse name must be at least 3 characters!");

        if (greenHouseName.Length > 20)
            throw new Exception("GreenHouse must be less than 21 characters!");
    }
    
    
    
    
}