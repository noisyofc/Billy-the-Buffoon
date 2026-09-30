using UnityEngine;

/// <summary>
/// Level unlock + best-score persistence. Safe to call from any scene
/// (does not depend on LevelSelectManager living in the Menu).
/// </summary>
public static class LevelProgress
{
    public const int FirstBiome = 1;
    public const int FirstLevel = 1;

    public static GameData Load()
    {
        GameData data = SaveSystem.LoadGame();
        EnsureStartingLevelUnlocked(data);
        return data;
    }

    public static void EnsureStartingLevelUnlocked(GameData data)
    {
        if (data == null)
            return;

        LevelData first = data.FindLevel(FirstBiome, FirstLevel);
        if (first == null)
            data.levels.Add(LevelData.CreateUnlocked(FirstBiome, FirstLevel));
        else
            first.isUnlocked = true;
    }

    public static void UnlockLevel(GameData data, int biomeNumber, int levelNumber)
    {
        if (data == null || biomeNumber < 1 || levelNumber < 1)
            return;

        LevelData level = data.FindLevel(biomeNumber, levelNumber);
        if (level == null)
        {
            data.levels.Add(LevelData.CreateUnlocked(biomeNumber, levelNumber));
            return;
        }

        level.isUnlocked = true;
    }

    /// <summary>
    /// Saves stats for the completed level and unlocks the next one in the biome.
    /// When finishing the last level of a biome (see <paramref name="levelsPerBiome"/>),
    /// also unlocks level 1 of the next biome.
    /// </summary>
    public static void SaveCompletedLevel(
        int biomeNumber,
        int levelNumber,
        float timeSeconds,
        int balloonsCollected,
        int totalBalloons,
        string grade,
        int levelsPerBiome = 12)
    {
        GameData data = Load();

        LevelData current = data.FindLevel(biomeNumber, levelNumber);
        if (current == null)
        {
            current = LevelData.CreateUnlocked(biomeNumber, levelNumber);
            data.levels.Add(current);
        }

        current.isUnlocked = true;

        // Keep the better time (lower is better); always refresh balloons/grade display fields.
        if (!current.HasTime || timeSeconds < current.bestTimeSeconds ||
            GradeRanking.IsAchievedGradeBetter(grade, current.grade))
        {
            current.bestTimeSeconds = timeSeconds;
            current.bestBalloons = balloonsCollected;
            current.totalBalloons = totalBalloons;
            current.grade = string.IsNullOrEmpty(grade) ? current.grade : grade;
        }
        else if (string.IsNullOrEmpty(current.grade) || current.grade == "N/A")
        {
            current.grade = grade;
            current.bestBalloons = balloonsCollected;
            current.totalBalloons = totalBalloons;
            if (!current.HasTime)
                current.bestTimeSeconds = timeSeconds;
        }

        // Next level in this biome
        UnlockLevel(data, biomeNumber, levelNumber + 1);

        // Gate to next biome
        if (levelsPerBiome > 0 && levelNumber >= levelsPerBiome)
            UnlockLevel(data, biomeNumber + 1, FirstLevel);

        SaveSystem.SaveGame(data);
        GameLog.Info($"Progress saved: completed {biomeNumber}_{levelNumber}, unlocked next.");
    }

    public static bool IsUnlocked(int biomeNumber, int levelNumber)
    {
        GameData data = Load();
        LevelData level = data.FindLevel(biomeNumber, levelNumber);
        return level != null && level.isUnlocked;
    }
}
