namespace Domain.Model
{
    public class GreenHouseDTO
    {
        public string GreenHouseId { get; set; }

        public string? GreenHouseName { get; set; }

        public string? Description { get; set; }

        public double? Temperature { get; set; }

        public double? LightIntensity { get; set; }

        public double? Co2Levels { get; set; }

        public double? Humidity { get; set; }

        public bool? IsWindowOpen { get; set; }
    }
}