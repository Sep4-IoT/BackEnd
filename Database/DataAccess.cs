using Domain.Model;

namespace Database;

public class DataAccess
{
    private readonly GreenHouseContext _context = new();

    public void ChangeWindowStatus(Window window)
    {
        window.changeStatus(); 

        _context.Windows.Update(window); 
        _context.SaveChanges();         
    }

    public void AddWindow(Window window)
    {
        _context.Windows.Add(window);
        _context.SaveChanges();
    }

    public List<Window> getAllWindows(Object obj)
    {
        return _context.Windows.ToList();
    }
    
     
    
}