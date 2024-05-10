using System.ComponentModel.DataAnnotations;

namespace Domain.Model;

public class User
{
    [Key]
    public int UserId { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    
    public ICollection<GreenHouse> GreenHouses { get; set; }


    public User(string username, string firstName, string lastName, string password)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
    }
    
    
    
    public User() {}
    
}