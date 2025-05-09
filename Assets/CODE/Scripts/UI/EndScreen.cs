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
    [Tooltip("Text element displaying the grade.")]
    public TextMeshProUGUI grade;   // UI element for showing time at the end screen
    [Tooltip("Image representing the end screen background.")]
    public GameObject endScreen;           // Chnvas component for the end screen background
    [Tooltip("The main canvas that will be disabled when the end screen shows.")]
    public GameObject canvas;         // Canvas to hide when end screen is shown

    public int currentBiome = 1; // Set this to the actual biome number for the level
    public int currentLevel = 1; // Set this to the actual level number within the biome

    public int totalBalloons = 9;

    [Tooltip("Grading")]
    private string gradeAchieved;

    [Tooltip("Grade treshholds")]
    public int bestTimeSeconds = 45;
    public int bestBalloons = 9;

    [Header("Game Data")]
    [Tooltip("Number of stars collected by the player.")]
    public int stars = 0;  // Number of stars collected
    [Tooltip("Total time taken by the player.")]
    public float time = 0f;  // Total time spent

    public string gradeCheck;
    public static EndScreen instance;

    public static bool endLevel;
    public static EndScreen Instance;

    public GameObject highScore;
    public TextMeshProUGUI highTime, highBalloons, highGrade;

    public enum Grade
    {
        D = 1,
        C = 2,
        B = 3,
        A = 4,
        S = 5,
        SPlus = 6  // Highest rank
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        // wy��cza bool, aby gra na starcie mog�a reagowa� na przycisk pauzy
        endLevel = false;
    }

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
            // w��cza bool, aby gra nie reagowa�a na przycisk pauzy
            endLevel = true;

            // Pause the game
            //Time.timeScale = 0;
            PlayerMovementAdvanced.Paused = true;

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

            //Grading system
            if (seconds <= bestTimeSeconds && stars >= bestBalloons)
            {
                gradeAchieved = "S+";
            }
            else if (seconds <= (bestTimeSeconds+5) && stars >= bestBalloons)
            {
                gradeAchieved = "S";
            }
            else if (seconds <= bestTimeSeconds && stars >= (bestBalloons-1))
            {
                gradeAchieved = "S";
            }
            else if (seconds <= (bestTimeSeconds+10) && stars >= bestBalloons)
            {
                gradeAchieved = "A";
            }
            else if (seconds <= bestTimeSeconds && stars >= (bestBalloons-3))
            {
                gradeAchieved = "A";
            }
            else if (seconds <= (bestTimeSeconds+15) && stars >= bestBalloons)
            {
                gradeAchieved = "B";
            }
            else if (seconds <= bestTimeSeconds && stars >= (bestBalloons-5))
            {
                gradeAchieved = "B";
            }
            else if (seconds <= (bestTimeSeconds+20) && stars >= bestBalloons)
            {
                gradeAchieved = "C";
            }
            else if (seconds <= bestTimeSeconds && stars >= (bestBalloons-7))
            {
                gradeAchieved = "C";
            }
            else
            {
                gradeAchieved = "D"; // Default to D if criteria for higher grades are not met
            }

            grade.text = gradeAchieved;
            
            GameData gameData = SaveSystem.LoadGame();
            LevelData levelData = gameData.levels.Find(level =>
                level.biomeNumber == currentBiome.ToString() && 
                level.levelNumber == currentLevel.ToString());
            if (levelData != null)
            {
                gradeCheck = levelData.grade;
                if (GradeComparison.IsAchievedGradeBetter(gradeAchieved, gradeCheck))
                {
                    LevelSelectManager.Instance.SaveLevelProgress(currentBiome, currentLevel, bestTime, balloonsCollected, gradeAchieved);
                    highScore.SetActive(true);
                    //highBalloons.text = starsEnd.text;
                    //highTime.text = timeEnd.text;
                    //highGrade.text = grade.text;

                    highTime.text = bestTime;
                    highBalloons.text = balloonsCollected;
                    highGrade.text = gradeAchieved;

                }
                else
                {
                    Debug.Log("Previous grade is better or equal.");
                    highScore.SetActive(false);
                    highGrade.text = levelData.grade;
                    highTime.text = levelData.bestTime;
                    highBalloons.text = levelData.bestBalloons;
                }
            }
            else
            {
                LevelSelectManager.Instance.SaveLevelProgress(currentBiome, currentLevel, bestTime, balloonsCollected, gradeAchieved);
                highScore.SetActive(true);
                highTime.text = bestTime;
                highBalloons.text = balloonsCollected;
                highGrade.text = gradeAchieved;
            }

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
            SceneTransitionManager.Instance.LoadScene(nextSceneIndex);
            Debug.Log("Loading next level: " + nextSceneIndex);
        }
        else
        {
            // Optional: handle end of levels (e.g., loop to start, show end screen)
            Debug.Log("No more levels. End of game.");
        }
    }

    public class GradeComparison
    {
        // Helper method to convert string to enum
        public static Grade GetGradeValue(string gradeString)
        {
            return gradeString switch
            {
                "S+" => Grade.SPlus,
                "S" => Grade.S,
                "A" => Grade.A,
                "B" => Grade.B,
                "C" => Grade.C,
                "D" => Grade.D,
                _ => Grade.D // Default to D if input is invalid
            };
        }

        // Method to check if the achieved grade is better than the saved grade
        public static bool IsAchievedGradeBetter(string gradeAchieved, string gradeCheck)
        {
            Grade achieved = GetGradeValue(gradeAchieved);
            Grade saved = GetGradeValue(gradeCheck);

            return achieved > saved; // Higher enum value means a better grade
        }
    }
}
