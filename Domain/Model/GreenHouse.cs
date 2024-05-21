using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Model
{
    public class GreenHouse
    {
        [BsonId] //mongodb key
        public string GreenHouseId { get; set; }

        public string? GreenHouseName { get; set; }

        public string? Description { get; set; }

        public double? Temperature { get; set; }

        public double? LightIntensity { get; set; }

        public double? Co2Levels { get; set; }

        public double? Humidity { get; set; }

        public bool? IsWindowOpen { get; set; }

        public GreenHouse(string? GreenHouseName, string? Description, double? Temperature, double? LightIntensity,
            double? Co2Levels, double? Humidity, bool? isWindowOpen)
        {
            this.GreenHouseName = GreenHouseName;
            this.Description = Description;
            this.Temperature = Temperature;
            this.LightIntensity = LightIntensity;
            this.Co2Levels = Co2Levels;
            this.Humidity = Humidity;
            IsWindowOpen = isWindowOpen;
        }

        public GreenHouse() {}
    }
}
