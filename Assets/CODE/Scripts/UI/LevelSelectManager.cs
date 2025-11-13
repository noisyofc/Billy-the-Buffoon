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

    public BiomeSelector biomeSelector; // Reference to the BiomeSelector
    private int selectedBiomeIndex;
    public List<LevelButton> levelButtons;

    public GameObject OptionsButton;
    public GameObject LevelButton;
    public GameObject QuitButton;
    public GameObject StartButton;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    public Camera mainCamera;
    private GameObject credits;

    public GameObject levelRender;
    public Sprite[] levelSprites;
    public string[] keys;
    //public MockMenuOptions mockMenuOptions;
    //public MockMenuSelectLevel mockMenuSelectLevel;

    // Dictionary to map level scene names to custom display names
    private Dictionary<string, string> levelDisplayNames = new Dictionary<string, string>
    {
        { "Level_1_1", "Forest Frenzy" },
        { "Level_1_2", "Cave Crawler" },
        { "Level_1_3", "Aerial Acribatics" },
        { "Level_1_4", "Wall Walker" },
        { "Level_1_5", "XYZ_1" },
        { "Level_1_6", "XYZ_2" },
        { "Level_1_7", "XYZ_3" },
        { "Level_1_8", "XYZ_4" },
        { "Level_1_9", "XYZ_5" },
        { "Level_1_10", "XYZ_6" },
        { "Level_1_11", "XYZ_7" },
        { "Level_1_12", "XYZ_8" },
        { "Level_2_1", "Sandy Spirit" },
        { "Level_2_2", "Desert Dash" },
        { "Level_2_3", "ZYX_1" },
        { "Level_2_4", "ZYX_2" },
        { "Level_2_5", "ZYX_3" },
        { "Level_2_6", "ZYX_4" },
        { "Level_2_7", "ZYX_5" },
        { "Level_2_8", "ZYX_6" },
        { "Level_2_9", "ZYX_7" },
        { "Level_2_10", "ZYX_8" },
        { "Level_2_11", "ZYX_9" },
        { "Level_2_12", "ZYX_10" },
        // Add more level mappings as needed
    };

    private Dictionary<string, Sprite> spriteDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        spriteDict = new Dictionary<string, Sprite>();

        // Safety check
        if (keys.Length != levelSprites.Length)
        {
            Debug.LogError("Keys and Sprites arrays must be the same length!");
            return;
        }

        for (int i = 0; i < levelSprites.Length; i++)
        {
            string key = keys[i];
            Sprite sprite = levelSprites[i];

            if (!string.IsNullOrEmpty(key) && sprite != null)
            {
                if (!spriteDict.ContainsKey(key))
                {
                    spriteDict.Add(key, sprite);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key skipped: {key}");
                }
            }
        }

        Debug.Log($"Loaded {spriteDict.Count} sprites into dictionary.");

        // ensure we have the LevelButton instances (don't overwrite inspector assignment)
        if (levelButtons == null || levelButtons.Count == 0)
        {
            levelButtons = new List<LevelButton>(FindObjectsOfType<LevelButton>());
            if (levelButtons.Count == 0)
                Debug.LogWarning("No LevelButton instances found in scene.");
            else
                EnsureLevelButtonsOrder(); // sort found buttons by UI sibling index (fallback: name)
        }
 
        // find biome selector early and set selectedBiomeIndex (safe null check)
        biomeSelector = FindObjectOfType<BiomeSelector>();
        if (biomeSelector != null)
            selectedBiomeIndex = biomeSelector.currentBiomeIndex;
        else
            selectedBiomeIndex = 0;

        // Initialize button biome/level numbers for the currently active layout
        // This ensures levelButton.biomeNumber/levelNumber are set before saving/unlocking
        UpdateLevelButtonsForBiome(selectedBiomeIndex);

        // NOW unlock levels (will write correct biome/level into save)
        UnlockAllLevels();

        credits = GameObject.FindGameObjectWithTag("Credits");

        playButton.interactable = false;
        playButton.onClick.AddListener(StartSelectedLevel);

        LoadLevelUnlockStatus(); // Load initial level unlock status
        DeselectCurrentLevel(); // Ensure no level is selected on canvas open

        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        //Debug.Log(selectedBiomeIndex);
        Debug.Log(biomeSelector.currentBiomeIndex);
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
        levelRender.GetComponent<Image>().sprite = levelSprites[0];
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

        if (!levelButton.isUnlocked)
        {
            Debug.Log("This level is locked!");
            levelNameText.text += " (Locked)";
        }
        
    }

    private void StartSelectedLevel()
    {
        if (selectedLevelButton != null && selectedLevelButton.isUnlocked)
        {
            int biomeToLoad = selectedLevelButton.biomeNumber;
            int levelToLoad = selectedLevelButton.levelNumber;
            string levelName = string.Format("Level_{0}_{1}", biomeToLoad, levelToLoad);
            SceneTransitionManager.Instance.LoadScene(levelName);

            Debug.Log("Starting Level: " + levelName);

            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovementAdvanced.Paused = false;
            //LoadSensitivity();
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
            if (levelButton.biomeNumber == selectedBiomeIndex)
            {
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

                levelButton.UpdateButtonState();
            }
            else
            {
                levelButton.isUnlocked = false;
                levelButton.UpdateButtonState();
            }
        }
    }

    // Method to update the level info UI based on selected level
    private void UpdateLevelInfoUI()
    {
        if (selectedLevelButton == null) return;

        string levelKey = $"Level_{selectedLevelButton.biomeNumber}_{selectedLevelButton.levelNumber}";

        if (levelDisplayNames.TryGetValue(levelKey, out string displayName))
        {
            levelNameText.text = displayName;
        }
        else
        {
            levelNameText.text = "Unknown Level";
        }

        GameData gameData = SaveSystem.LoadGame();
        LevelData levelData = gameData.levels.Find(level =>
            level.biomeNumber == selectedLevelButton.biomeNumber.ToString() &&
            level.levelNumber == selectedLevelButton.levelNumber.ToString());

        if (levelData != null)
        {
            if (spriteDict.TryGetValue(levelKey, out Sprite result))
                {
                    levelRender.GetComponent<Image>().sprite = result;
                }
            else
            {
                Debug.LogWarning($"Sprite not found for key: {levelKey}");
            }
            bestTimeText.text = $"Best Time: {levelData.bestTime}";
            bestBalloonsText.text = $"Collected: {levelData.bestBalloons}";
            gradeText.text = $"Grade: {levelData.grade}";
        }
        else
        {
            if (spriteDict.TryGetValue(levelKey, out Sprite result))
            {
                levelRender.GetComponent<Image>().sprite = result;
            }
            else
            {
                Debug.LogWarning($"Sprite not found for key: {levelKey}");
            }
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
        credits.SetActive(true);
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



    public void UnlockAllLevels()
    {
        // Load the current game data
        GameData gameData = SaveSystem.LoadGame();

        if (keys == null || keys.Length == 0)
        {
            Debug.LogWarning("UnlockAllLevels: keys array is empty. Populate 'keys' in the inspector with scene names like 'Level_1_1'.");
            return;
        }

        foreach (string key in keys)
        {
            if (string.IsNullOrEmpty(key)) continue;

            // Expect keys in format "Level_{biome}_{level}"
            string[] parts = key.Split('_');
            if (parts.Length < 3) continue;

            if (!int.TryParse(parts[1], out int biomeNum) || !int.TryParse(parts[2], out int levelNum))
                continue;

            string biomeStr = biomeNum.ToString();
            string levelStr = levelNum.ToString();

            LevelData levelData = gameData.levels.Find(level =>
                level.biomeNumber == biomeStr && level.levelNumber == levelStr);

            if (levelData == null)
            {
                levelData = new LevelData
                {
                    biomeNumber = biomeStr,
                    levelNumber = levelStr,
                    isUnlocked = "true",
                    bestTime = "N/A",
                    bestBalloons = "N/A",
                    grade = "N/A"
                };
                gameData.levels.Add(levelData);
            }
            else
            {
                levelData.isUnlocked = "true";
            }
        }

        SaveSystem.SaveGame(gameData);

        // Update any LevelButton instances in the scene so UI reflects unlocked state
        foreach (LevelButton lb in FindObjectsOfType<LevelButton>())
        {
            lb.isUnlocked = true;
            lb.UpdateButtonState();
        }

        Debug.Log("All levels have been unlocked (based on keys array).");
    }

    public void LockLevelsPast1_4()
    {
        // Load the current game data
        GameData gameData = SaveSystem.LoadGame();

        // Iterate through all level buttons
        foreach (LevelButton levelButton in FindObjectsOfType<LevelButton>())
        {
            // Check if the level is past Level_1_4
            if (levelButton.biomeNumber > 1 || (levelButton.biomeNumber == 1 && levelButton.levelNumber > 4))
            {
                // Lock the level
                levelButton.isUnlocked = false;
                levelButton.UpdateButtonState();

                // Update the game data to reflect the locked status
                LevelData levelData = gameData.levels.Find(level =>
                    level.biomeNumber == levelButton.biomeNumber.ToString() &&
                    level.levelNumber == levelButton.levelNumber.ToString());

                if (levelData != null)
                {
                    levelData.isUnlocked = "false";
                }
                levelButton.isUnlocked = false;
                levelButton.UpdateButtonState();
            }
        }

        // Save the updated game data
        SaveSystem.SaveGame(gameData);
        

        Debug.Log("All levels past Level_1_4 have been locked.");
    }

    // Sort levelButtons so their indices match the visual order in the hierarchy.
    // Called only when we auto-populated levelButtons.
    private void EnsureLevelButtonsOrder()
    {
        if (levelButtons == null || levelButtons.Count <= 1) return;

        // If all buttons share the same parent, sort by sibling index (UI layout order)
        Transform parent = levelButtons[0].transform.parent;
        bool sameParent = true;
        foreach (var b in levelButtons)
            if (b.transform.parent != parent) { sameParent = false; break; }

        if (sameParent)
        {
            levelButtons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            return;
        }

        // Fallback: try to sort by trailing number in the GameObject name (LevelButton_1, btn5, etc.)
        levelButtons.Sort((a, b) => ParseTrailingNumber(a.gameObject.name).CompareTo(ParseTrailingNumber(b.gameObject.name)));
    }

    private int ParseTrailingNumber(string name)
    {
        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(name[i]))
            {
                string digits = name.Substring(i + 1);
                if (int.TryParse(digits, out int v)) return v;
                break;
            }
        }
        // fallback: first numeric sequence
        var m = System.Text.RegularExpressions.Regex.Match(name, @"\d+");
        if (m.Success && int.TryParse(m.Value, out int val)) return val;
        return int.MaxValue; // put at the end
    }

}
