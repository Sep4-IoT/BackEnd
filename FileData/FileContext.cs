using System.Text.Json;
using Domain.Model;

namespace FileData;

public class FileContext
{
    private const string filePath = "data.json";
    private DataContainer? dataContainer;

    public ICollection<GreenHouse> GreenHouses
    {
        get
        {
            LoadData();
            return dataContainer!.GreenHouses;
        }
    }
    
    public ICollection<User> Users
    {
        get
        {
            LoadData();
            return dataContainer!.Users;
        }
    }
    
    
    private void LoadData()
    {
        if (dataContainer != null) return;
    
        if (!File.Exists(filePath))
        {
            dataContainer = new ()
            {
                Users = new List<User>(),
                GreenHouses = new List<GreenHouse>()
            };
            return;
        }
        string content = File.ReadAllText(filePath);
        dataContainer = JsonSerializer.Deserialize<DataContainer>(content);
    }
    
    public void SaveChanges()
    {
        string serialized = JsonSerializer.Serialize(dataContainer, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(filePath, serialized);
        dataContainer = null;
    }
}