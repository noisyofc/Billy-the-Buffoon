using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int biomeNumber;
    public int levelNumber;
    public bool isUnlocked;
    public float bestTimeSeconds = -1f;
    public int bestBalloons = -1;
    public int totalBalloons = -1;
    public string grade = "N/A";

    public bool HasTime => bestTimeSeconds >= 0f;
    public bool HasBalloons => bestBalloons >= 0;

    public string FormatTime()
    {
        if (!HasTime)
            return "N/A";

        int minutes = Mathf.FloorToInt(bestTimeSeconds / 60f);
        int seconds = Mathf.FloorToInt(bestTimeSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((bestTimeSeconds * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public string FormatBalloons()
    {
        if (!HasBalloons)
            return "N/A";
        if (totalBalloons > 0)
            return $"{bestBalloons}/{totalBalloons}";
        return bestBalloons.ToString();
    }

    public static LevelData CreateUnlocked(int biome, int level)
    {
        return new LevelData
        {
            biomeNumber = biome,
            levelNumber = level,
            isUnlocked = true,
            bestTimeSeconds = -1f,
            bestBalloons = -1,
            totalBalloons = -1,
            grade = "N/A"
        };
    }
}
