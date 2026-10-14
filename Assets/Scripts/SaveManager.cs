using System.IO;
using UnityEngine;

namespace zombie_survival
{
    public class SaveManager
    {
        private string filePath;

        public SaveManager()
        {
            filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        }
        public void Save(SaveData data)
        {
            string jsonString = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonString);
        }

        public SaveData Load()
        {
            if (File.Exists(filePath))
            {
                string jsonText = File.ReadAllText(filePath);
                SaveData data = JsonUtility.FromJson<SaveData>(jsonText);
                return data ?? new SaveData();
            }
            else
            {
                return new SaveData();
            }
        }
    }
}