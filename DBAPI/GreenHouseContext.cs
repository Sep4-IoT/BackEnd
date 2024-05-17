using Domain.Model;
using MongoDB.Driver;

namespace WebAPI.Data
{
    public class GreenHouseContext
    {
        private readonly IMongoDatabase _database;

        public GreenHouseContext()
        {
            var client = new MongoClient("mongodb://mongo:27017");
            _database = client.GetDatabase("GreenHouseDb");
        }

        public IMongoCollection<GreenHouse> GreenHouses => _database.GetCollection<GreenHouse>("GreenHouses");
    }
}