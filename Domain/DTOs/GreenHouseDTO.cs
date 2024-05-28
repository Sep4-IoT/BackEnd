using System.Text.Json.Serialization;

namespace Domain.Model
{
    public class GreenHouseDTO
    {
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

        public bool? IsWindowOpen { get; set; }
    }
}