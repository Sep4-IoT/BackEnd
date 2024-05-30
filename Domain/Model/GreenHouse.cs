using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System;

namespace Domain.Model
{
    public class GreenHouse
    {
        [BsonId] // MongoDB key
        [JsonPropertyName("Id")]
        public int GreenHouseId { get; set; }

        [JsonPropertyName("Name")]
        public string? GreenHouseName { get; set; }

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName("LightIntensity")]
        public double? LightIntensity { get; set; }

        [JsonPropertyName("Co2Levels")]
        public double? Co2Levels { get; set; }

        [JsonPropertyName("Humidity")]
        public double? Humidity { get; set; }

        [JsonPropertyName("IsWindowOpen")]
        public bool? IsWindowOpen { get; set; }

        public DateTime Date { get; set; }  

        public GreenHouse(string? GreenHouseName, string? Description, double? Temperature, double? LightIntensity,
            double? Co2Levels, double? Humidity, bool? isWindowOpen, DateTime date)
        {
            this.GreenHouseName = GreenHouseName;
            this.Description = Description;
            this.Temperature = Temperature;
            this.LightIntensity = LightIntensity;
            this.Co2Levels = Co2Levels;
            this.Humidity = Humidity;
            IsWindowOpen = isWindowOpen;
            Date = date;
        }

        public GreenHouse() {}
    }
}