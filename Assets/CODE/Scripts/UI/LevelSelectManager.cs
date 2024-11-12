using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;
    private LevelButton selectedLevelButton;

    public Button playButton;

    // Reference to UI Text elements
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI bestBalloonsText;
    public TextMeshProUGUI gradeText;

    private BiomeSelector biomeSelector; // Reference to the BiomeSelector
    private int selectedBiomeIndex;
    public List<LevelButton> levelButtons;

    public GameObject OptionsButton;
    public GameObject LevelButton;
    public GameObject QuitButton;
    public GameObject StartButton;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    public Camera mainCamera;

    //public MockMenuOptions mockMenuOptions;
    //public MockMenuSelectLevel mockMenuSelectLevel;

    // Dictionary to map level scene names to custom display names
    private Dictionary<string, string> levelDisplayNames = new Dictionary<string, string>
    {
        { "Level_1_1", "Aerial Acrobatics" },
        { "Level_1_2", "Mountain Climber" },
        { "Level_1_3", "Desert Dash" },
        { "Level_1_4", "Sandy Sprint" },
        { "Level_2_1", "Boulder Breaker" },
        // Add more level mappings as needed
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //levelButtons = new List<LevelButton>(FindObjectsOfType<LevelButton>());

        biomeSelector = FindObjectOfType<BiomeSelector>();
        selectedBiomeIndex = biomeSelector.currentBiomeIndex;

        playButton.interactable = false;
        playButton.onClick.AddListener(StartSelectedLevel);

        LoadLevelUnlockStatus(); // Load initial level unlock status
        DeselectCurrentLevel(); // Ensure no level is selected on canvas open

        //mockMenuSelectLevel = GetComponent<MockMenuSelectLevel>();
        //mockMenuOptions = GetComponent<MockMenuOptions>();

        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);
    }

    private void Update()
    {
        // Check if the biome has changed
        if (selectedBiomeIndex != biomeSelector.currentBiomeIndex)
        {
            // Deselect the currently selected level when biome changes
            DeselectCurrentLevel();

            selectedBiomeIndex = biomeSelector.currentBiomeIndex;
            LoadLevelUnlockStatus();
        }
    }

    // Method to deselect the currently selected level button
    private void DeselectCurrentLevel()
    {
        if (selectedLevelButton != null)
        {
            // Deselect the level button and disable its background
            selectedLevelButton.SetSelected(false); // Deactivates the background within SetSelected
            selectedLevelButton = null;
        }

        // Disable the play button since no level is selected
        playButton.interactable = false;

        // Clear level info UI
        levelNameText.text = "Select a Level";
        bestTimeText.text = "Best Time: N/A";
        bestBalloonsText.text = "Collected: N/A";
        gradeText.text = "Grade: N/A";
    }


    // Method to select a level and update UI
    public void SelectLevel(LevelButton levelButton)
    {
        DeselectCurrentLevel();

        // Select the new level
        selectedLevelButton = levelButton;
        selectedLevelButton.SetSelected(true);

        playButton.interactable = levelButton.isUnlocked;

        // Display the selected level information on the UI
        UpdateLevelInfoUI();
    }

    private void StartSelectedLevel()
    {
        if (selectedLevelButton != null && selectedLevelButton.isUnlocked)
        {
            int biomeToLoad = selectedLevelButton.biomeNumber;
            int levelToLoad = selectedLevelButton.levelNumber;
            string levelName = string.Format("Level_{0}_{1}", biomeToLoad, levelToLoad);
            SceneManager.LoadScene(levelName);

            Debug.Log("Starting Level: " + levelName);
        }
        else
        {
            Debug.LogWarning("Cannot start a locked level!");
        }
    }

    private void LoadLevelUnlockStatus()
    {
        GameData gameData = SaveSystem.LoadGame();

foreach (LevelButton levelButton in FindObjectsOfType<LevelButton>())
{
    // Only process buttons for the currently selected biome
    if (levelButton.biomeNumber == selectedBiomeIndex)
    {
        LevelData levelData = gameData.levels.Find(level => 
            level.biomeNumber == levelButton.biomeNumber.ToString() &&
            level.levelNumber == levelButton.levelNumber.ToString());

        if (levelData != null)
        {
            levelButton.isUnlocked = levelData.isUnlocked == "true";
            levelButton.UpdateButtonState();
        }
        else
        {
            levelButton.isUnlocked = false;
            levelButton.UpdateButtonState();
        }
    }
    else
    {
        // Lock buttons not in the selected biome
        levelButton.isUnlocked = false;
        levelButton.UpdateButtonState();
    }
}
    }

    // Method to update the level info UI based on selected level
    private void UpdateLevelInfoUI()
    {
        if (selectedLevelButton == null) return;

        // Construct the level key based on biome and level numbers
        string levelKey = $"Level_{selectedLevelButton.biomeNumber}_{selectedLevelButton.levelNumber}";

        // Set the level name using the display names dictionary
        if (levelDisplayNames.TryGetValue(levelKey, out string displayName))
        {
            levelNameText.text = displayName;
        }
        else
        {
            levelNameText.text = "Unknown Level"; // Default text if no display name is found
        }

        // Load game data to retrieve best time, balloons, and grade
        GameData gameData = SaveSystem.LoadGame();
        LevelData levelData = gameData.levels.Find(level =>
            level.biomeNumber == selectedLevelButton.biomeNumber.ToString() &&
            level.levelNumber == selectedLevelButton.levelNumber.ToString());

        if (levelData != null)
        {
            bestTimeText.text = $"Best Time: {levelData.bestTime}";
            bestBalloonsText.text = $"Collected: {levelData.bestBalloons}";
            gradeText.text = $"Grade: {levelData.grade}";
        }
        else
        {
            bestTimeText.text = "Best Time: N/A";
            bestBalloonsText.text = "Collected: N/A";
            gradeText.text = "Grade: N/A";
        }
    }

    public void SaveLevelProgress(int biomeNumber, int levelNumber, string timeElapsed, string balloonsCollected, string grade)
    {
        GameData gameData = SaveSystem.LoadGame();

        LevelData levelData = gameData.levels.Find(level =>
            level.biomeNumber == biomeNumber.ToString() &&
            level.levelNumber == levelNumber.ToString());

        if (levelData == null)
        {
            levelData = new LevelData
            {
                biomeNumber = biomeNumber.ToString(),
                levelNumber = levelNumber.ToString(),
                isUnlocked = "true",
                bestTime = timeElapsed,
                bestBalloons = balloonsCollected,
                grade = grade
            };
            gameData.levels.Add(levelData);
        }
        else
        {
            levelData.isUnlocked = "true";
            levelData.bestTime = timeElapsed;
            levelData.bestBalloons = balloonsCollected;
            levelData.grade = grade;
        }

        SaveSystem.SaveGame(gameData);
    }

    public void goBack()
    {
        DeselectCurrentLevel();
        gameObject.SetActive(false);
        LevelButton.GetComponent<MockMenuSelectLevel>().enabled = true;
        LevelButton.GetComponent<Collider>().enabled = true;
        QuitButton.GetComponent<MockMenuExit>().enabled = true;
        QuitButton.GetComponent<Collider>().enabled = true;
        StartButton.GetComponent<MockMenuStartGame>().enabled = true;
        StartButton.GetComponent<Collider>().enabled = true;
        OptionsButton.GetComponent<MockMenuOptions>().enabled = true;
        OptionsButton.GetComponent<Collider>().enabled = true;
        MockMenuSelectLevel.isMouseOver = false;
        MockMenuOptions.isMouseOver = false;
        depthOfField.active = false;
    }

    public void LockAllLevels()
    {
        // Deselect any selected level
        DeselectCurrentLevel();

        // Lock all levels
        foreach (LevelButton levelButton in FindObjectsOfType<LevelButton>())
        {
            levelButton.isUnlocked = false;
            levelButton.UpdateButtonState();
        }

        Debug.Log("All levels have been locked.");
    }

    public void UpdateLevelButtonsForBiome(int biomeIndex)
    {
        DeselectCurrentLevel(); // Clear current selection and background

        selectedBiomeIndex = biomeIndex;

        // Update each button to reflect the new biome and level numbers
        for (int i = 0; i < levelButtons.Count; i++)
        {
            LevelButton levelButton = levelButtons[i];
            levelButton.biomeNumber = biomeIndex; // Set the biome number
            levelButton.levelNumber = i + 1;      // Set level number based on index (1, 2, 3, 4)

            // Check if this level is unlocked based on saved data
            GameData gameData = SaveSystem.LoadGame();
            LevelData levelData = gameData.levels.Find(level =>
                level.biomeNumber == levelButton.biomeNumber.ToString() &&
                level.levelNumber == levelButton.levelNumber.ToString());

            if (levelData != null)
            {
                levelButton.isUnlocked = levelData.isUnlocked == "true";
            }
            else
            {
                levelButton.isUnlocked = false;
            }

            levelButton.UpdateButtonState(); // Update button appearance
        }
    }

}
