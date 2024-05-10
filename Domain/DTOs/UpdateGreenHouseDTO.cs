namespace Domain.DTOs;

public class UpdateGreenHouseDTO
{
    public int GreenHouseID { get; }
    public string? GreenHouseName { get; set; }
    public string? Description { get; set; }
    public bool? IsWindowOpen { get; set; }


    public UpdateGreenHouseDTO(int greenHouseId, string? greenHouseName, string? description, bool? isWindowOpen)
    {
        GreenHouseID = greenHouseId;
        GreenHouseName = greenHouseName;
        Description = description;
        IsWindowOpen = isWindowOpen;
    }

    public UpdateGreenHouseDTO()
    {
        
    }
}