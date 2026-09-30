using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// In-game pause panel controller. Settings persistence is shared with OptionsManager via PlayerPrefs.
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    public Slider sensitivitySlider;
    public float defaultSensitivity = 1.0f;
    public string mainMenuScene = "GAME/Scenes/Menu";
    public GameObject optionsCanvas;
    public GameObject mainUI;
    public Camera mainCamera;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    private float currentSensitivity;

    void Start()
    {
        currentSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = currentSensitivity;
            sensitivitySlider.onValueChanged.AddListener(_ => OnSensitivityChange());
        }

        if (mainCamera != null)
        {
            postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
            if (postProcessVolume != null)
                postProcessVolume.profile.TryGetSettings(out depthOfField);
        }
    }

    public void PauseGame()
    {
        GameState.IsPaused = true;
        Time.timeScale = 0;
        if (optionsCanvas != null) optionsCanvas.SetActive(true);
        if (mainUI != null) mainUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (depthOfField != null) depthOfField.active = true;
    }

    public void ResumeGame()
    {
        GameState.IsPaused = false;
        Time.timeScale = 1;
        if (optionsCanvas != null) optionsCanvas.SetActive(false);
        if (mainUI != null) mainUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (depthOfField != null) depthOfField.active = false;
        currentSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);
    }

    public void OnSensitivityChange()
    {
        if (sensitivitySlider == null)
            return;

        currentSensitivity = sensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", currentSensitivity);
        PlayerPrefs.Save();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        GameState.ResetForGameplay();
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void restartLevel()
    {
        ResumeGame();
        if (MusicManager.instance != null)
            MusicManager.instance.SaveMusicTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
