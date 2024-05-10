namespace Domain.DTOs;

public class SearchGreenHouseDTO
{
    public int? GreenHouseID { get; }

    public SearchGreenHouseDTO(int? greenHouseId)
    {
        GreenHouseID = greenHouseId;
    }
}