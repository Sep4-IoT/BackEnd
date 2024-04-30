using Domain.Model;
    
namespace Database;

public class DataAccess
{
    private readonly GreenHouseContext _context = new();

    public void ChangeWindowStatus(GreenHouse greenhouse)
    {
        greenhouse.changeWindowStatus(); 
        
        _context.Greenhouses.Update(greenhouse); 
        _context.SaveChanges();         
    }

    public bool getWindowStatus(GreenHouse greenHouse)
    {
        return greenHouse.IsWindowOpen ?? false; // return false if null
    }
    
}