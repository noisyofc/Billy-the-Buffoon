using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Text element displaying the number of collected stars.")]
    public TextMeshProUGUI starsEnd;  // UI element for showing stars at the end screen
    [Tooltip("Text element displaying the total time.")]
    public TextMeshProUGUI timeEnd;   // UI element for showing time at the end screen
    [Tooltip("Image representing the end screen background.")]
    public Image endScreen;           // Image component for the end screen background
    [Tooltip("The main canvas that will be disabled when the end screen shows.")]
    public GameObject canvas;         // Canvas to hide when end screen is shown

    public int currentBiome = 1; // Set this to the actual biome number for the level
    public int currentLevel = 1; // Set this to the actual level number within the biome

    public int totalBalloons = 9;

    public string gradeAchieved = "A+";

    [Header("Game Data")]
    [Tooltip("Number of stars collected by the player.")]
    public int stars = 0;  // Number of stars collected
    [Tooltip("Total time taken by the player.")]
    public float time = 0f;  // Total time spent

    private void Update()
    {
        // Update the stars display
        starsEnd.text = string.Format("{0}/{1}", stars.ToString(), totalBalloons);

        // Format the time to MM:SS and update the time display
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        timeEnd.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    /// <summary>
    /// When the player collides with the end zone, trigger the end screen.
    /// </summary>
    /// <param name="other">Collider of the object that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Pause the game
            Time.timeScale = 0;

            // Unlock and make the cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Hide the gameplay canvas and show the end screen
            canvas.SetActive(false);
            endScreen.gameObject.SetActive(true);

            // Get stars collected and time from other game components
            stars = CountStars.stars;
            time = Timer.timeElapsed;

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 100) % 100);
            string bestTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

            string balloonsCollected = string.Format("{0}/{1}", stars.ToString(), totalBalloons);

            gradeAchieved = "T-";

            LevelSelectManager.Instance.SaveLevelProgress(currentBiome, currentLevel, bestTime, balloonsCollected, gradeAchieved);

        }
    }

    public void StartNextLevel()
    {
        // Get the current active scene's build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the next scene index
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the build settings range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log("Loading next level: " + nextSceneIndex);
        }
        else
        {
            // Optional: handle end of levels (e.g., loop to start, show end screen)
            Debug.Log("No more levels. End of game.");
        }
    }
}
