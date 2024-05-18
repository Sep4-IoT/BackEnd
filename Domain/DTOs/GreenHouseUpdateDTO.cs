namespace Domain.DTOs;

public class GreenHouseUpdateDTO
{
    public int GreenHouseId { get; set; }
    public string? GreenHouseName { get; set; }
    public string? Description { get; set; }
    public bool? IsWindowOpen { get; set; }
}