using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;
    private LevelButton selectedLevelButton;

    public Button playButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playButton.interactable = false;
        playButton.onClick.AddListener(StartSelectedLevel);
    }

    public void SelectLevel(LevelButton levelButton)
    {
        // Deselect the previously selected level
        if (selectedLevelButton != null)
        {
            selectedLevelButton.SetSelected(false);
        }

        // Select the new level
        selectedLevelButton = levelButton;
        selectedLevelButton.SetSelected(true);

        // Enable the play button
        playButton.interactable = true;
    }

    private void StartSelectedLevel()
    {
        if (selectedLevelButton != null)
        {
            int levelToLoad = selectedLevelButton.levelNumber;
            string levelName = string.Format("Level_1_{0}", levelToLoad);
            SceneManager.LoadScene(levelName);
            Debug.Log("Starting Level: " + levelToLoad);
            // Implement your level-loading logic here, e.g., using SceneManager.LoadScene(levelToLoad);
        }
    }
}
