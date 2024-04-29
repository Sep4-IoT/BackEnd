using Domain.DTOs;
using Domain.Model;

namespace Application.LogicInterfaces;

public interface IGreenHouseLogic
{ 
    Task<IEnumerable<GreenHouse>> GetAsync(SearchGreenHouseDTO searchParameters);
    Task<GreenHouse> CreateAsync(GreenHouseCreationDTO greenHouseCreation);

    Task UpdateAsync(int greenHouseId, UpdateGreenHouseDTO updateGreenHouse);

}