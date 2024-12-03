using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public string loadingSceneName = "LoadingScene";
    public AsyncOperation CurrentLoadingOperation { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method for loading scenes by name
    public void LoadScene(string targetSceneName)
    {
        StartCoroutine(LoadSceneWithLoadingScreen(targetSceneName));
    }

    // Overloaded method for loading scenes by index
    public void LoadScene(int targetSceneIndex)
    {
        StartCoroutine(LoadSceneWithLoadingScreen(targetSceneIndex));
    }

    private IEnumerator LoadSceneWithLoadingScreen(string targetSceneName)
    {
        // Load the loading screen
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(loadingSceneName);

        while (!loadingScene.isDone)
        {
            yield return null;
        }

        // Start loading the target scene
        CurrentLoadingOperation = SceneManager.LoadSceneAsync(targetSceneName);
        CurrentLoadingOperation.allowSceneActivation = false;

        // Wait until the target scene is ready
        while (!CurrentLoadingOperation.isDone)
        {
            if (CurrentLoadingOperation.progress >= 0.9f)
            {
                yield break; // Stop the coroutine; the loading screen will handle activation
            }
            yield return null;
        }
    }

    private IEnumerator LoadSceneWithLoadingScreen(int targetSceneIndex)
    {
        // Load the loading screen
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(loadingSceneName);

        while (!loadingScene.isDone)
        {
            yield return null;
        }

        // Start loading the target scene
        CurrentLoadingOperation = SceneManager.LoadSceneAsync(targetSceneIndex);
        CurrentLoadingOperation.allowSceneActivation = false;

        // Wait until the target scene is ready
        while (!CurrentLoadingOperation.isDone)
        {
            if (CurrentLoadingOperation.progress >= 0.9f)
            {
                yield break; // Stop the coroutine; the loading screen will handle activation
            }
            yield return null;
        }
    }
}
