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

    private MockMenuSelectLevel _levelMenu;
    private Collider _levelCollider;
    private MockMenuExit _quitMenu;
    private Collider _quitCollider;
    private MockMenuStartGame _startMenu;
    private Collider _startCollider;
    private MockMenuOptions _optionsMenu;
    private Collider _optionsCollider;

    public GameObject levelRender;
    public Sprite[] levelSprites;
    public string[] keys;
    //public MockMenuOptions mockMenuOptions;
    //public MockMenuSelectLevel mockMenuSelectLevel;

    // Dictionary to map level scene names to custom display names
    public LevelsDictionary levelsDictionary;
    public Dictionary<string, string> levelDisplayNames;


    private Dictionary<string, Sprite> spriteDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        levelDisplayNames = levelsDictionary.levelDisplayNames;
    }

    private void Start()
    {
        spriteDict = new Dictionary<string, Sprite>();

        // Safety check
        if (keys.Length != levelSprites.Length)
        {
            GameLog.Error("Keys and Sprites arrays must be the same length!");
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
                    GameLog.Warn($"Duplicate key skipped: {key}");
                }
            }
        }

        GameLog.Info($"Loaded {spriteDict.Count} sprites into dictionary.");

        // ensure we have the LevelButton instances (don't overwrite inspector assignment)
        if (levelButtons == null || levelButtons.Count == 0)
        {
            levelButtons = new List<LevelButton>(FindObjectsOfType<LevelButton>());
            if (levelButtons.Count == 0)
                GameLog.Warn("No LevelButton instances found in scene.");
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

        credits = GameObject.FindGameObjectWithTag("Credits");

        CacheMenuButtons();

        playButton.interactable = false;
        playButton.onClick.AddListener(StartSelectedLevel);

        LoadLevelUnlockStatus(); // Load initial level unlock status
        DeselectCurrentLevel(); // Ensure no level is selected on canvas open

        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);

        gameObject.SetActive(false);
    }

    void CacheMenuButtons()
    {
        if (LevelButton != null)
        {
            _levelMenu = LevelButton.GetComponent<MockMenuSelectLevel>();
            _levelCollider = LevelButton.GetComponent<Collider>();
        }
        if (QuitButton != null)
        {
            _quitMenu = QuitButton.GetComponent<MockMenuExit>();
            _quitCollider = QuitButton.GetComponent<Collider>();
        }
        if (StartButton != null)
        {
            _startMenu = StartButton.GetComponent<MockMenuStartGame>();
            _startCollider = StartButton.GetComponent<Collider>();
        }
        if (OptionsButton != null)
        {
            _optionsMenu = OptionsButton.GetComponent<MockMenuOptions>();
            _optionsCollider = OptionsButton.GetComponent<Collider>();
        }
    }

    private void Update()
    {
        //GameLog.Info(selectedBiomeIndex);
        //GameLog.Info(biomeSelector.currentBiomeIndex);
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
            GameLog.Info("This level is locked!");
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

            GameLog.Info("Starting Level: " + levelName);

            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovementAdvanced.Paused = false;
            //LoadSensitivity();
        }
        else
        {
            GameLog.Warn("Cannot start a locked level!");
        }
    }

    private void LoadLevelUnlockStatus()
    {
        GameData gameData = LevelProgress.Load();
        SaveSystem.SaveGame(gameData); // persist default unlock of 1_1 for new saves

        foreach (LevelButton levelButton in FindObjectsOfType<LevelButton>())
        {
            if (levelButton.biomeNumber == selectedBiomeIndex)
            {
                LevelData levelData = gameData.FindLevel(levelButton.biomeNumber, levelButton.levelNumber);
                levelButton.isUnlocked = levelData != null && levelData.isUnlocked;
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

        GameData gameData = LevelProgress.Load();
        LevelData levelData = gameData.FindLevel(selectedLevelButton.biomeNumber, selectedLevelButton.levelNumber);

        if (spriteDict.TryGetValue(levelKey, out Sprite result))
            levelRender.GetComponent<Image>().sprite = result;
        else
            GameLog.Warn($"Sprite not found for key: {levelKey}");

        if (levelData != null)
        {
            bestTimeText.text = $"Best Time: {levelData.FormatTime()}";
            bestBalloonsText.text = $"Collected: {levelData.FormatBalloons()}";
            gradeText.text = $"Grade: {levelData.grade}";
        }
        else
        {
            bestTimeText.text = "Best Time: N/A";
            bestBalloonsText.text = "Collected: N/A";
            gradeText.text = "Grade: N/A";
        }
    }

    public void SaveLevelProgress(int biomeNumber, int levelNumber, float timeSeconds, int balloonsCollected, int totalBalloons, string grade)
    {
        LevelProgress.SaveCompletedLevel(
            biomeNumber,
            levelNumber,
            timeSeconds,
            balloonsCollected,
            totalBalloons,
            grade);
    }

    public void goBack()
    {
        DeselectCurrentLevel();
        gameObject.SetActive(false);

        if (_levelMenu != null) _levelMenu.enabled = true;
        if (_levelCollider != null) _levelCollider.enabled = true;
        if (_quitMenu != null) _quitMenu.enabled = true;
        if (_quitCollider != null) _quitCollider.enabled = true;
        if (_startMenu != null) _startMenu.enabled = true;
        if (_startCollider != null) _startCollider.enabled = true;
        if (_optionsMenu != null) _optionsMenu.enabled = true;
        if (_optionsCollider != null) _optionsCollider.enabled = true;

        MockMenuSelectLevel.isMouseOver = false;
        MockMenuOptions.isMouseOver = false;
        if (depthOfField != null)
            depthOfField.active = false;
        if (credits != null)
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

        GameLog.Info("All levels have been locked.");
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
            GameData gameData = LevelProgress.Load();
            LevelData levelData = gameData.FindLevel(levelButton.biomeNumber, levelButton.levelNumber);
            levelButton.isUnlocked = levelData != null && levelData.isUnlocked;

            levelButton.UpdateButtonState(); // Update button appearance
        }
    }



    /// <summary>
    /// Debug helper — unlocks every level in <see cref="keys"/>.
    /// Not called at runtime; use the component context menu in the Editor.
    /// </summary>
    [ContextMenu("DEBUG/Unlock All Levels")]
    public void UnlockAllLevels()
    {
        // Load the current game data
        GameData gameData = SaveSystem.LoadGame();

        if (keys == null || keys.Length == 0)
        {
            GameLog.Warn("UnlockAllLevels: keys array is empty. Populate 'keys' in the inspector with scene names like 'Level_1_1'.");
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

            LevelData levelData = gameData.FindLevel(biomeNum, levelNum);

            if (levelData == null)
            {
                gameData.levels.Add(LevelData.CreateUnlocked(biomeNum, levelNum));
            }
            else
            {
                levelData.isUnlocked = true;
            }
        }

        SaveSystem.SaveGame(gameData);

        // Update any LevelButton instances in the scene so UI reflects unlocked state
        foreach (LevelButton lb in FindObjectsOfType<LevelButton>())
        {
            lb.isUnlocked = true;
            lb.UpdateButtonState();
        }

        GameLog.Info("All levels have been unlocked (based on keys array).");
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
                LevelData levelData = gameData.FindLevel(levelButton.biomeNumber, levelButton.levelNumber);

                if (levelData != null)
                {
                    levelData.isUnlocked = false;
                }
                levelButton.isUnlocked = false;
                levelButton.UpdateButtonState();
            }
        }

        // Save the updated game data
        SaveSystem.SaveGame(gameData);
        

        GameLog.Info("All levels past Level_1_4 have been locked.");
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
