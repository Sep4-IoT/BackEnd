using Application.DAOInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Model;

namespace Application.Logic;

public class UserLogic : IUserLogic
{

    private readonly IUserDAO userDao;

    public UserLogic(IUserDAO userDao)
    {
        this.userDao = userDao;
    }
    public async Task<User> CreateAsync(UserCreationDTO userCreationDto)
    {
        User? existing = await userDao.GetByUsernameAsync(userCreationDto.Username);
        if (existing != null)
            throw new Exception("Username already taken!");

        //ValidateData(dto);
        User toCreate = new User
        {
            Username = userCreationDto.Username,
            FirstName = userCreationDto.FirstName,
            LastName = userCreationDto.LastName,
            Password = userCreationDto.Password
            
        };
    
        User created = await userDao.CreateAsync(toCreate);
    
        return created;
    }

    public Task<IEnumerable<User>> GetAsync(SearchUserDTO searchParameters)
    {
        return userDao.GetAsync(searchParameters);
    }
}