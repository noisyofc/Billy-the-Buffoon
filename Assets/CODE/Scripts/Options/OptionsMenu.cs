using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public Slider sensitivitySlider;
    public float defaultSensitivity = 1.0f;
    private float currentSensitivity;
    public string mainMenuScene = "GAME/Scenes/Menu";
    public GameObject optionsCanvas; 
    public GameObject mainUI;

    void Start()
    {
        // Load saved sensitivity or use default
        LoadSensitivity();
        // Set the slider's value to the loaded sensitivity
        sensitivitySlider.value = currentSensitivity;
        // Add a listener to handle slider value change
        sensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivityChange(); });
    }

    public void OnSensitivityChange()
    {
        currentSensitivity = sensitivitySlider.value;
        SaveSensitivity(); // Save the updated sensitivity
    }

    public void SaveSensitivity()
    {
        // Save the current sensitivity to PlayerPrefs
        PlayerPrefs.SetFloat("MouseSensitivity", currentSensitivity);
        PlayerPrefs.Save(); // Ensure the value is written to disk
        Debug.Log("Sensitivity Saved: " + currentSensitivity);
    }

    public void LoadSensitivity()
    {
        // Load the saved sensitivity or use the default value if it doesn't exist
        currentSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);
        Debug.Log("Sensitivity Loaded: " + currentSensitivity);
    }

    public void ExitGame()
    {
        // Save the current sensitivity
        Application.Quit();
    }

    public void MainMenu()
    {
        // Save the current sensitivity
        SceneManager.LoadScene(mainMenuScene);
    }

    public void ResumeGame()
    {
        // Save the current sensitivity
        Time.timeScale = 1;
        optionsCanvas.SetActive(false);
        mainUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerMovementAdvanced.Paused = false;
        LoadSensitivity();
    }
}
