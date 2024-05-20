using Domain.Model;
using MongoDB.Driver;
using WebAPI.Data;

namespace WebAPI.Repositories
{
    public class GreenHouseRepository
    {
        private readonly GreenHouseContext _context;

        public GreenHouseRepository(GreenHouseContext context)
        {
            _context = context;
        }

        public async Task<List<GreenHouse>> GetAllAsync()
        {
            return await _context.GreenHouses.Find(_ => true).ToListAsync();
        }

        public async Task<GreenHouse> GetByIdAsync(string id)
        {
            return await _context.GreenHouses.Find(g => g.GreenHouseId == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(GreenHouse greenHouse)
        {
            await _context.GreenHouses.InsertOneAsync(greenHouse);
        }

        public async Task UpdateAsync(GreenHouse greenHouse)
        {
            await _context.GreenHouses.ReplaceOneAsync(g => g.GreenHouseId == greenHouse.GreenHouseId, greenHouse);
        }

        public async Task UpdateFieldAsync(string id, string field, object value)
        {
            var update = Builders<GreenHouse>.Update.Set(field, value);
            await _context.GreenHouses.UpdateOneAsync(g => g.GreenHouseId == id, update);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.GreenHouses.DeleteOneAsync(g => g.GreenHouseId == id);
        }
    }
}