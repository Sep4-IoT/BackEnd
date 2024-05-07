using Application.DAOInterfaces;
using Domain.DTOs;
using Domain.Model;

namespace FileData.DAOs;

public class GreenHouseFileDAO : IGreenHouseDAO
{
    private readonly FileContext context;
    
    public GreenHouseFileDAO(FileContext context)
    {
        this.context = context;
    }
    
    public Task<IEnumerable<GreenHouse>> GetAsync(SearchGreenHouseDTO searchParameters)
    {
        IEnumerable<GreenHouse> greenHouses = context.GreenHouses.AsEnumerable();
        if (searchParameters.GreenHouseID != null)
        {
           greenHouses = context.GreenHouses.Where(g => g.GreenHouseId == searchParameters.GreenHouseID);
        }

        return Task.FromResult(greenHouses);
    }

    public Task<GreenHouse> CreateAsync(GreenHouse greenHouse)
    {
        int greenHouseId = 1;
        if (context.GreenHouses.Any())
        {
            greenHouseId = context.GreenHouses.Max(g => g.GreenHouseId);
            greenHouseId++;
        }

        greenHouse.GreenHouseId = greenHouseId;

        context.GreenHouses.Add(greenHouse);
        context.SaveChanges();

        return Task.FromResult(greenHouse);
    }

    public Task<GreenHouse?> GetByNameAsync(string name)
    {
       GreenHouse? existing = context.GreenHouses.FirstOrDefault(g =>
            g.GreenHouseName.Equals(name, StringComparison.OrdinalIgnoreCase)
        );
        return Task.FromResult(existing);
    }

    public async Task<UpdateGreenHouseDTO> UpdateAsync(UpdateGreenHouseDTO updateGreenHouseDto)
    {
        GreenHouse? existing = context.GreenHouses.FirstOrDefault(g => g.GreenHouseId == updateGreenHouseDto.GreenHouseID);
        if (existing == null)
        {
            throw new Exception($"GreenHouse with id {updateGreenHouseDto.GreenHouseID} does not exist!");
        }

        existing.GreenHouseName = updateGreenHouseDto.GreenHouseName;
        existing.Description = updateGreenHouseDto.Description;
        existing.IsWindowOpen = updateGreenHouseDto.IsWindowOpen;
        
        await Task.Run(() => context.SaveChanges());
        
        return updateGreenHouseDto;
    }
    
    public Task<GreenHouse?> GetByIdAsync(int id)
    {
        GreenHouse? existing = context.GreenHouses.FirstOrDefault(g => g.GreenHouseId == id);
        return Task.FromResult(existing);
    }

    public Task<IEnumerable<GreenHouse>> GetByOwnerIdAsync(int ownerId)
    {
        
        IEnumerable<GreenHouse> greenHouses = context.GreenHouses.AsEnumerable();
        if (ownerId != null)
        {
            greenHouses = context.GreenHouses.Where(g => g.OwnerId == ownerId);
        }

        return Task.FromResult(greenHouses);
    }
}