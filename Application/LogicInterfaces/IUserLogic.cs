using Domain.DTOs;
using Domain.Model;

namespace Application.LogicInterfaces;

public interface IUserLogic
{
    Task<User> CreateAsync(UserCreationDTO userCreationDto);
    Task<IEnumerable<User>> GetAsync(SearchUserDTO searchParameters);
}