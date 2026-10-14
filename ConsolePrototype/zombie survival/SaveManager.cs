using System.IO;
using System.Text.Json;

namespace zombie_survival;

public class SaveManager
{
    private string filePath = "gamedata.json";
    public void Save(SaveData data)
    {
        string jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(filePath, jsonString);
    }

    public SaveData Load()
    {
        if (File.Exists(filePath))
        {
            string jsonText = File.ReadAllText(filePath);
            SaveData data = JsonSerializer.Deserialize<SaveData>(jsonText);
            return data ?? new SaveData();
        }
        else
        {
            return new SaveData();
        }
    }
}