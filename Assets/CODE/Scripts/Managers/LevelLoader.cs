using UnityEngine;

/// <summary>
/// Optional level-load hook for future Addressables (P4).
/// Today this forwards to SceneTransitionManager; swap implementation when groups exist.
/// </summary>
public static class LevelLoader
{
    public static void LoadByName(string sceneName)
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public static void LoadByBuildIndex(int buildIndex)
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene(buildIndex);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }
}
