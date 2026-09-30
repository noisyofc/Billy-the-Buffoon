using NUnit.Framework;
using UnityEngine;

public class GameStateTests
{
    [SetUp]
    public void SetUp()
    {
        GameState.ResetForGameplay();
        GameState.IsTutorialActive = false;
    }

    [Test]
    public void ResetForGameplay_ClearsPauseDeathAndLevelEnd()
    {
        GameState.IsPaused = true;
        GameState.IsPlayerDead = true;
        GameState.IsLevelEnded = true;

        GameState.ResetForGameplay();

        Assert.IsFalse(GameState.IsPaused);
        Assert.IsFalse(GameState.IsPlayerDead);
        Assert.IsFalse(GameState.IsLevelEnded);
    }

    [Test]
    public void CanPause_False_WhenLevelEnded()
    {
        GameState.IsLevelEnded = true;
        Assert.IsFalse(GameState.CanPause);
    }

    [Test]
    public void CanPause_False_WhenTutorialActive()
    {
        GameState.IsTutorialActive = true;
        Assert.IsFalse(GameState.CanPause);
    }

    [Test]
    public void CanPause_True_WhenIdle()
    {
        Assert.IsTrue(GameState.CanPause);
    }
}

public class LevelDataTests
{
    [Test]
    public void FormatTime_ReturnsNA_WhenUnset()
    {
        var data = new LevelData();
        Assert.AreEqual("N/A", data.FormatTime());
    }

    [Test]
    public void FormatTime_FormatsMinutesSecondsMilliseconds()
    {
        var data = new LevelData { bestTimeSeconds = 90f };
        Assert.AreEqual("01:30:00", data.FormatTime());
    }

    [Test]
    public void FormatBalloons_UsesTotalWhenPresent()
    {
        var data = new LevelData { bestBalloons = 3, totalBalloons = 9 };
        Assert.AreEqual("3/9", data.FormatBalloons());
    }

    [Test]
    public void CreateUnlocked_SetsFlags()
    {
        var data = LevelData.CreateUnlocked(2, 4);
        Assert.AreEqual(2, data.biomeNumber);
        Assert.AreEqual(4, data.levelNumber);
        Assert.IsTrue(data.isUnlocked);
        Assert.IsFalse(data.HasTime);
    }
}

public class GradeComparisonTests
{
    [Test]
    public void SPlus_IsBetterThan_A()
    {
        Assert.IsTrue(GradeRanking.IsAchievedGradeBetter("S+", "A"));
    }

    [Test]
    public void B_IsNotBetterThan_A()
    {
        Assert.IsFalse(GradeRanking.IsAchievedGradeBetter("B", "A"));
    }

    [Test]
    public void SameGrade_IsNotBetter()
    {
        Assert.IsFalse(GradeRanking.IsAchievedGradeBetter("S", "S"));
    }
}

public class LevelProgressTests
{
    [Test]
    public void EnsureStartingLevelUnlocked_CreatesLevel1_1()
    {
        var data = new GameData();
        LevelProgress.EnsureStartingLevelUnlocked(data);

        LevelData first = data.FindLevel(1, 1);
        Assert.IsNotNull(first);
        Assert.IsTrue(first.isUnlocked);
    }

    [Test]
    public void UnlockLevel_AddsMissingEntry()
    {
        var data = new GameData();
        LevelProgress.UnlockLevel(data, 1, 3);

        LevelData level = data.FindLevel(1, 3);
        Assert.IsNotNull(level);
        Assert.IsTrue(level.isUnlocked);
    }
}

public class GameDataFindTests
{
    [Test]
    public void FindLevel_ReturnsMatchingEntry()
    {
        var data = new GameData();
        data.levels.Add(LevelData.CreateUnlocked(1, 2));
        data.levels.Add(LevelData.CreateUnlocked(1, 3));

        LevelData found = data.FindLevel(1, 3);
        Assert.IsNotNull(found);
        Assert.AreEqual(3, found.levelNumber);
    }

    [Test]
    public void FindLevel_ReturnsNull_WhenMissing()
    {
        var data = new GameData();
        Assert.IsNull(data.FindLevel(9, 9));
    }
}
