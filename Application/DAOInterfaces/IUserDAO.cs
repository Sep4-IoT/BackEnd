using Domain.DTOs;
using Domain.Model;

namespace Application.DAOInterfaces;

public interface IUserDAO
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAsync(SearchUserDTO searchParameters);
}