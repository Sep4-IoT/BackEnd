using Domain.Model;

namespace Domain.DTOs;

public class SearchUserDTO
{
    public int? UserId { get; }

    public SearchUserDTO(int? userId)
    {
        UserId = userId;
    }
}