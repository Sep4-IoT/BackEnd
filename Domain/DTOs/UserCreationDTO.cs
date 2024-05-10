namespace Domain.DTOs;

public class UserCreationDTO
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }


    public UserCreationDTO(string username, string firstName, string lastName, string password)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
    }
    
    
}