using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string filePath = Application.persistentDataPath + "/saveData.json";

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            // Return new GameData if no save file exists
            return new GameData();
        }
    }
}
