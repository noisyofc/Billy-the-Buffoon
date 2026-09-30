using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

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
    public static EndScreen instance
    {
        get => Instance;
        set => Instance = value;
    }

    /// <summary>Forwards to <see cref="GameState.IsLevelEnded"/>.</summary>
    public static bool endLevel
    {
        get => GameState.IsLevelEnded;
        set => GameState.IsLevelEnded = value;
    }

    public static EndScreen Instance;

    public GameObject highScore;
    public TextMeshProUGUI highTime, highBalloons, highGrade;

    public LevelsDictionary levelsDictionary;
    public TMP_Text levelNameText;
    private string levelKey;
    public Dictionary<string, string> levelDisplayNames;

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
        GameState.IsLevelEnded = false;
        levelDisplayNames = levelsDictionary != null ? levelsDictionary.levelDisplayNames : null;
        levelKey = SceneManager.GetActiveScene().name;
        TryApplyBiomeLevelFromSceneName(levelKey);
    }

    /// <summary>
    /// Prefer scene name Level_{biome}_{level} over stale prefab defaults.
    /// </summary>
    private void TryApplyBiomeLevelFromSceneName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return;

        string[] parts = sceneName.Split('_');
        if (parts.Length < 3)
            return;

        if (int.TryParse(parts[1], out int biome) && int.TryParse(parts[2], out int level))
        {
            currentBiome = biome;
            currentLevel = level;
        }
    }

    private void RefreshEndUi()
    {
        if (starsEnd != null)
            starsEnd.text = $"{stars}/{totalBalloons}";

        if (timeEnd != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 100) % 100);
            timeEnd.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }

        if (levelNameText == null)
            return;

        if (levelDisplayNames != null && levelDisplayNames.TryGetValue(levelKey, out string displayName))
            levelNameText.text = displayName;
        else
            levelNameText.text = "Unknown Level";
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
            GameState.IsLevelEnded = true;
            GameState.IsPaused = true;

            // Unlock and make the cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Hide the gameplay canvas and show the end screen
            canvas.SetActive(false);
            endScreen.gameObject.SetActive(true);

            // Get stars collected and time from other game components
            stars = CountStars.stars;
            time = Timer.timeElapsed;
            RefreshEndUi();

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 100) % 100);

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

            string bestTimeDisplay = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
            string balloonsDisplay = $"{stars}/{totalBalloons}";

            GameData gameData = LevelProgress.Load();
            LevelData levelData = gameData.FindLevel(currentBiome, currentLevel);
            gradeCheck = levelData != null ? levelData.grade : "N/A";

            bool isNewBest = levelData == null ||
                             !levelData.HasTime ||
                             GradeRanking.IsAchievedGradeBetter(gradeAchieved, gradeCheck) ||
                             time < levelData.bestTimeSeconds;

            // Always persist via Core — LevelSelectManager does not exist in level scenes.
            LevelProgress.SaveCompletedLevel(
                currentBiome,
                currentLevel,
                time,
                stars,
                totalBalloons,
                gradeAchieved);

            if (isNewBest)
            {
                highScore.SetActive(true);
                highTime.text = bestTimeDisplay;
                highBalloons.text = balloonsDisplay;
                highGrade.text = gradeAchieved;
            }
            else if (levelData != null)
            {
                highScore.SetActive(false);
                highGrade.text = levelData.grade;
                highTime.text = levelData.FormatTime();
                highBalloons.text = levelData.FormatBalloons();
            }
            else
            {
                highScore.SetActive(true);
                highTime.text = bestTimeDisplay;
                highBalloons.text = balloonsDisplay;
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
            GameLog.Info("Loading next level: " + nextSceneIndex);
        }
        else
        {
            // Optional: handle end of levels (e.g., loop to start, show end screen)
            GameLog.Info("No more levels. End of game.");
        }
    }
}
