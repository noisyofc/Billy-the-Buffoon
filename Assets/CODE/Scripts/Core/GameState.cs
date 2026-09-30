/// <summary>
/// Single source of truth for cross-system gameplay flags.
/// Lives in Game.Core — no references to gameplay MonoBehaviours.
/// </summary>
public static class GameState
{
    public static bool IsPaused { get; set; }
    public static bool IsPlayerDead { get; set; }
    public static bool IsLevelEnded { get; set; }
    public static bool IsTutorialActive { get; set; }

    public static bool CanPause => !IsLevelEnded && !IsTutorialActive;

    public static void ResetForGameplay()
    {
        IsPaused = false;
        IsPlayerDead = false;
        IsLevelEnded = false;
        // Tutorial flag is owned by TutorialManager for the session
    }
}
