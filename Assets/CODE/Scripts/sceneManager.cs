using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    [Header("Scenes to Load")]
    [Tooltip("Path for the test level scene.")]
    public string testLevelScene = "GAME/Scenes/Level 0 - TEST_LEVEL";
    [Tooltip("Path for the main game scene (Level 1).")]
    public string startGameScene = "GAME/Scenes/Level 1 - Borys";
    [Tooltip("Path for the game load scene (Level 3).")]
    public string loadGameScene = "GAME/Scenes/Level 3 - Borys";
    [Tooltip("Path for the next level scene (Level 2).")]
    public string nextLevelScene = "GAME/Scenes/Level 2 - Borys";
    [Tooltip("Path for the main menu scene.")]
    public string mainMenuScene = "GAME/Scenes/Menu";

    // Load the test level scene
    public void LoadTestLevel()
    {
        SceneManager.LoadScene(testLevelScene);
        Time.timeScale = 1;  // Ensure time is running normally
        PlayerMovementAdvanced.Paused = false;
    }

    // Start the game by loading Level 1
    public void StartGame()
    {
        SceneManager.LoadScene(startGameScene);
        Time.timeScale = 1;
        PlayerMovementAdvanced.Paused = false;
    }

    // Load the next level (Level 2)
    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevelScene);
        Time.timeScale = 1;
        PlayerMovementAdvanced.Paused = false;
    }

    // Load a saved game (Level 3)
    public void LoadGame()
    {
        SceneManager.LoadScene(loadGameScene);
        Time.timeScale = 1;
        PlayerMovementAdvanced.Paused = false;
    }

    // Exit the game application
    public void ExitGame()
    {
        Application.Quit();
    }

    // Load the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1;
        PlayerMovementAdvanced.Paused = false;
    }
}
