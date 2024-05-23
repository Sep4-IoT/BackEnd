using Domain.Model;
using MongoDB.Driver;
using WebAPI.Data;

namespace WebAPI.Repositories
{
    public class GreenHouseDateListRepository
    {
        private readonly GreenHouseContext _context;

        public GreenHouseDateListRepository(GreenHouseContext context)
        {
            _context = context;
        }

        public async Task<GreenHouseDateList> GetByIdAsync(string id)
        {
            return await _context.GreenHouseDateLists.Find(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(GreenHouseDateList greenHouseDateList)
        {
            await _context.GreenHouseDateLists.InsertOneAsync(greenHouseDateList);
        }

        public async Task UpdateAsync(GreenHouseDateList greenHouseDateList)
        {
            await _context.GreenHouseDateLists.ReplaceOneAsync(g => g.Id == greenHouseDateList.Id, greenHouseDateList);
        }
    }
}