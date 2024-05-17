using Application.DAOInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess;

public class GreenHouseEfcDAO : IGreenHouseDAO
{
    private readonly GreenHouseContext context;

    public GreenHouseEfcDAO(GreenHouseContext context)
    {
        this.context = context;
    }
    public async Task<IEnumerable<GreenHouse>> GetAsync(SearchGreenHouseDTO searchParameters)
    {
        IQueryable<GreenHouse> query = context.GreenHouses.AsQueryable();
        if (searchParameters.GreenHouseID != null)
        {
            query = query.Where(g => g.GreenHouseId == (searchParameters.GreenHouseID));
        }

        IEnumerable<GreenHouse> result = await query.ToListAsync();
        return result;
        
    }

    public async Task<GreenHouse> CreateAsync(GreenHouse greenHouse)
    {
        EntityEntry<GreenHouse> added = await context.GreenHouses.AddAsync(greenHouse);
        await context.SaveChangesAsync();
        return added.Entity;
    }

    public async Task<GreenHouse?> GetByNameAsync(string userName)
    {
        GreenHouse? existing = await context.GreenHouses.FirstOrDefaultAsync(g =>
            g.GreenHouseName.ToLower().Equals(userName.ToLower())
        );
        return existing;
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
        
        await Task.Run(() => context.SaveChangesAsync());
        
        return updateGreenHouseDto;
    }
    

    public async Task<GreenHouse> GetByIdAsync(int id)
    {
        GreenHouse? greenHouse = await context.GreenHouses.FindAsync(id);
        return greenHouse;
    }

    /*public async Task<IEnumerable<GreenHouse>> GetByOwnerIdAsync(int ownerId)
    {
        IQueryable<GreenHouse> usersQuery = context.GreenHouses.AsQueryable();
        if (ownerId != null)
        {
            usersQuery = usersQuery.Where(g => g.OwnerId == ownerId);
        }

        IEnumerable<GreenHouse> result = await usersQuery.ToListAsync();
        return result;
    }*/
}