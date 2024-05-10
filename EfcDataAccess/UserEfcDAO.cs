using Application.DAOInterfaces;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess;

public class UserEfcDAO : IUserDAO
{
    
    private readonly GreenHouseContext context;

    public UserEfcDAO(GreenHouseContext context)
    {
        this.context = context;
    }
    public async Task<User> CreateAsync(User user)
    {
        EntityEntry<User> newUser = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return newUser.Entity;
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        User? existing = await context.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower().Equals(userName.ToLower())
        );
        return existing;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        User? user = await context.Users.FindAsync(id);
        return user;
    }

    public async Task<IEnumerable<User>> GetAsync(SearchUserDTO searchParameters)
    {
        IQueryable<User> usersQuery = context.Users.AsQueryable();
        if (searchParameters.UserId != null)
        {
            usersQuery = usersQuery.Where(u => u.UserId == (searchParameters.UserId));
        }

        IEnumerable<User> result = await usersQuery.ToListAsync();
        return result;
    }
}