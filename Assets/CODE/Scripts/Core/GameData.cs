using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public const int CurrentVersion = 2;

    public int version = CurrentVersion;
    public List<LevelData> levels = new List<LevelData>();

    public LevelData FindLevel(int biomeNumber, int levelNumber)
    {
        return levels.Find(level =>
            level.biomeNumber == biomeNumber &&
            level.levelNumber == levelNumber);
    }
}
