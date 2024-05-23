using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Domain.Model
{
    public class GreenHouseDateList
    {
        [BsonId] // MongoDB key
        public string Id { get; set; }

        public List<GreenHouse> GreenHouses { get; set; } = new List<GreenHouse>();

        public GreenHouseDateList(string id)
        {
            Id = id;
        }
    }
}