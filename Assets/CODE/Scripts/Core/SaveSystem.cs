using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string FilePath = Application.persistentDataPath + "/saveData.json";

    public static void SaveGame(GameData data)
    {
        if (data == null)
            data = new GameData();

        data.version = GameData.CurrentVersion;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    public static GameData LoadGame()
    {
        if (!File.Exists(FilePath))
            return new GameData();

        string json = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(json))
            return new GameData();

        if (IsLegacySave(json))
        {
            GameData migrated = MigrateFromLegacy(json);
            SaveGame(migrated);
            return migrated;
        }

        GameData data = JsonUtility.FromJson<GameData>(json);
        if (data == null)
            return new GameData();

        if (data.levels == null)
            data.levels = new List<LevelData>();

        if (data.version < GameData.CurrentVersion)
        {
            data.version = GameData.CurrentVersion;
            SaveGame(data);
        }

        return data;
    }

    public static void DeleteSave()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    private static bool IsLegacySave(string json)
    {
        // v1 used string fields like "isUnlocked": "true"
        return !json.Contains("\"version\"") ||
               json.Contains("\"isUnlocked\": \"true\"") ||
               json.Contains("\"isUnlocked\": \"false\"") ||
               json.Contains("\"isUnlocked\":\"true\"") ||
               json.Contains("\"isUnlocked\":\"false\"");
    }

    private static GameData MigrateFromLegacy(string json)
    {
        var legacy = JsonUtility.FromJson<LegacyGameData>(json);
        var migrated = new GameData { version = GameData.CurrentVersion };

        if (legacy?.levels == null)
            return migrated;

        foreach (LegacyLevelData old in legacy.levels)
        {
            if (old == null)
                continue;

            int.TryParse(old.biomeNumber, out int biome);
            int.TryParse(old.levelNumber, out int level);

            var entry = new LevelData
            {
                biomeNumber = biome,
                levelNumber = level,
                isUnlocked = string.Equals(old.isUnlocked, "true", StringComparison.OrdinalIgnoreCase),
                grade = string.IsNullOrEmpty(old.grade) ? "N/A" : old.grade,
                bestTimeSeconds = ParseLegacyTime(old.bestTime),
                bestBalloons = -1,
                totalBalloons = -1
            };

            ParseLegacyBalloons(old.bestBalloons, out entry.bestBalloons, out entry.totalBalloons);
            migrated.levels.Add(entry);
        }

        return migrated;
    }

    private static float ParseLegacyTime(string bestTime)
    {
        if (string.IsNullOrEmpty(bestTime) || bestTime == "N/A")
            return -1f;

        string[] parts = bestTime.Split(':');
        if (parts.Length < 2)
            return -1f;

        if (!int.TryParse(parts[0], out int minutes) || !int.TryParse(parts[1], out int seconds))
            return -1f;

        int milliseconds = 0;
        if (parts.Length >= 3)
            int.TryParse(parts[2], out milliseconds);

        return minutes * 60f + seconds + milliseconds / 100f;
    }

    private static void ParseLegacyBalloons(string value, out int balloons, out int total)
    {
        balloons = -1;
        total = -1;
        if (string.IsNullOrEmpty(value) || value == "N/A")
            return;

        string[] parts = value.Split('/');
        if (parts.Length >= 1 && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int b))
            balloons = b;
        if (parts.Length >= 2 && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int t))
            total = t;
    }

    [Serializable]
    private class LegacyGameData
    {
        public List<LegacyLevelData> levels;
    }

    [Serializable]
    private class LegacyLevelData
    {
        public string biomeNumber;
        public string levelNumber;
        public string isUnlocked;
        public string bestTime;
        public string bestBalloons;
        public string grade;
    }
}
