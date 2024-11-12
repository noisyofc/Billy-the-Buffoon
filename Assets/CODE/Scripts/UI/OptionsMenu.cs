using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class OptionsMenu : MonoBehaviour
{

    public Slider sensitivitySlider;
    public float defaultSensitivity = 1.0f;
    private float currentSensitivity;
    public string mainMenuScene = "GAME/Scenes/Menu";
    public GameObject optionsCanvas; 
    public GameObject mainUI;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    public Camera mainCamera;
    
    public static bool Paused { get; private set; } = false;  // Centralized pause state

    void Start()
    {
        LoadSensitivity();
        sensitivitySlider.value = currentSensitivity;
        sensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivityChange(); });

        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);
    }


    public void PauseGame()
    {
        Paused = true;
        Time.timeScale = 0;
        optionsCanvas.SetActive(true);
        mainUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerMovementAdvanced.Paused = true;
        depthOfField.active = true;
    }

    public void ResumeGame()
    {
        Paused = false;
        Time.timeScale = 1;
        optionsCanvas.SetActive(false);
        mainUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerMovementAdvanced.Paused = false;
        depthOfField.active = false;
        LoadSensitivity();
    }

    public void OnSensitivityChange()
    {
        currentSensitivity = sensitivitySlider.value;
        SaveSensitivity();
    }

    public void SaveSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", currentSensitivity);
        PlayerPrefs.Save();
        Debug.Log("Sensitivity Saved: " + currentSensitivity);
    }

    public void LoadSensitivity()
    {
        currentSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);
        Debug.Log("Sensitivity Loaded: " + currentSensitivity);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1;
        PlayerMovementAdvanced.Paused = false;
    }

    public void restartLevel()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
