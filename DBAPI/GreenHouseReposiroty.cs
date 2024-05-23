using Domain.Model;
using MongoDB.Bson;
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
            var greenHouseLists = await _context.GreenHouseDateLists.Find(_ => true).ToListAsync();
            return greenHouseLists.SelectMany(g => g.GreenHouses).ToList();
        }

        public async Task<GreenHouse> GetByIdAsync(string id)
        {
            var greenHouseList = await _context.GreenHouseDateLists.Find(g => g.Id == id).FirstOrDefaultAsync();
            return greenHouseList?.GreenHouses.OrderByDescending(gh => gh.Date).FirstOrDefault();
        }

        public async Task AddAsync(GreenHouse greenHouse)
        {
            var greenHouseList = await _context.GreenHouseDateLists.Find(g => g.Id == greenHouse.GreenHouseId).FirstOrDefaultAsync();
            if (greenHouseList == null)
            {
                greenHouseList = new GreenHouseDateList(greenHouse.GreenHouseId);
                greenHouseList.GreenHouses.Add(greenHouse);
                await _context.GreenHouseDateLists.InsertOneAsync(greenHouseList);
            }
            else
            {
                greenHouseList.GreenHouses.Add(greenHouse);
                await _context.GreenHouseDateLists.ReplaceOneAsync(g => g.Id == greenHouseList.Id, greenHouseList);
            }
        }

        public async Task UpdateAsync(GreenHouse greenHouse)
        {
            var greenHouseList = await _context.GreenHouseDateLists.Find(g => g.Id == greenHouse.GreenHouseId).FirstOrDefaultAsync();
            if (greenHouseList != null)
            {
                var index = greenHouseList.GreenHouses.FindIndex(g => g.Date == greenHouse.Date);
                if (index >= 0)
                {
                    greenHouseList.GreenHouses[index] = greenHouse;
                    await _context.GreenHouseDateLists.ReplaceOneAsync(g => g.Id == greenHouseList.Id, greenHouseList);
                }
            }
        }

        public async Task UpdateFieldAsync(string id, string field, object value)
        {
            var filter = Builders<GreenHouseDateList>.Filter.And(
                Builders<GreenHouseDateList>.Filter.Eq(g => g.Id, id),
                Builders<GreenHouseDateList>.Filter.ElemMatch(g => g.GreenHouses, gh => gh.GreenHouseId == id)
            );

            var update = Builders<GreenHouseDateList>.Update.Set($"GreenHouses.$[gh].{field}", value);
            var arrayFilters = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("gh.GreenHouseId", id)) };

            var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

            await _context.GreenHouseDateLists.UpdateOneAsync(filter, update, updateOptions);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.GreenHouseDateLists.DeleteOneAsync(g => g.Id == id);
        }
    }
}
