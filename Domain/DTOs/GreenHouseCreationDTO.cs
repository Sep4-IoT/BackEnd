using Domain.Model;

namespace Domain.DTOs;

public class GreenHouseCreationDTO
{
    public string GreenHouseName {get;}
    public int OwnerId { get; set; }
    public string? Description { get; set; }
    public double? Temperature { get; set; }
    public double? LightIntensity { get; set; }
    public double? Co2Levels { get; set; }
    public double? Humidity { get; set; }
    public bool? IsWindowOpen { get; set; }

    public GreenHouseCreationDTO(int ownerId, string greenHouseName, string? description, double? temperature, double? lightIntensity,
        double? co2Levels, double? humidity, bool? isWindowOpen)
    {
        OwnerId = ownerId;
        GreenHouseName = greenHouseName;
        Description = description;
        Temperature = temperature;
        LightIntensity = lightIntensity;
        Co2Levels = co2Levels;
        Humidity = humidity;
        IsWindowOpen = isWindowOpen;
    }
    
    
    
}