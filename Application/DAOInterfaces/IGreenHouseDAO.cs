using Domain.DTOs;
using Domain.Model;

namespace Application.DAOInterfaces;

public interface IGreenHouseDAO
{
    Task<IEnumerable<GreenHouse>> GetAsync(SearchGreenHouseDTO searchParameters);
    Task<GreenHouse> CreateAsync(GreenHouse greenHouse);
    Task<GreenHouse?> GetByNameAsync(string userName);
    Task<UpdateGreenHouseDTO> UpdateAsync(UpdateGreenHouseDTO updateGreenHouseDto);
    Task<GreenHouse> GetByIdAsync(int id);
    Task<IEnumerable<GreenHouse>> GetByOwnerIdAsync(int ownerId);
    Task UpdateTemperature(GreenHouse greenHouse);

}