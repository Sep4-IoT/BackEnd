using Domain.DTOs;
using Domain.Model;

namespace Application.LogicInterfaces;

public interface IGreenHouseLogic
{ 
    Task<IEnumerable<GreenHouse>> GetAsync(SearchGreenHouseDTO searchParameters);
    Task<GreenHouse> CreateAsync(GreenHouseCreationDTO greenHouseCreation);
    Task<GreenHouse> GetByIdAsync(int id);
    Task<UpdateGreenHouseDTO> UpdateAsync(UpdateGreenHouseDTO updateGreenHouse);
    Task<IEnumerable<GreenHouse>> GetByOwnerIdAsync(int userId);
    Task UpdateTemperature(int greenhouseId, double temperature);

}