using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task UpdateTemperature(GreenHouse greenHouse)
    {
        try
        {
            Console.WriteLine($"Updating temperature for GreenHouse ID {greenHouse.GreenHouseId} to {greenHouse.Temperature}");
            context.GreenHouses.Update(greenHouse);
            await context.SaveChangesAsync();
            Console.WriteLine("Temperature updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating temperature: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }
    
    public async Task TestQuery()
    {
        try
        {
            var greenHouses = await context.GreenHouses.ToListAsync();
            foreach (var gh in greenHouses)
            {
                Console.WriteLine($"GreenHouse ID: {gh.GreenHouseId}, Name: {gh.GreenHouseName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running test query: {ex.Message}");
        }
    }

    
    public async Task<GreenHouse> GetByIdAsync(int greenhouseId)
    {
        try
        {
            var greenHouse = await context.GreenHouses.FindAsync(greenhouseId);
            if (greenHouse == null)
            {
                Console.WriteLine($"No GreenHouse found with ID {greenhouseId}");
            }
            return greenHouse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching GreenHouse by ID {greenhouseId}: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<GreenHouse>> GetByOwnerIdAsync(int userId)
    {
        IQueryable<GreenHouse> usersQuery = context.GreenHouses.AsQueryable();
        if (userId != null)
        {
            usersQuery = usersQuery.Where(g => g.UserId == userId);
        }

        IEnumerable<GreenHouse> result = await usersQuery.ToListAsync();
        return result;
    }
    
    
}