using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public GameObject generalPanel;
    public GameObject videoPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;

    public Slider sensitivitySliderMouse;
    public float defaultSensitivity = 1.0f;
    private float currentSensitivity;

    public void Start()
    {
        LoadSensitivity();
        sensitivitySliderMouse.value = currentSensitivity;
        sensitivitySliderMouse.onValueChanged.AddListener(delegate { OnSensitivityChange(); });
    }

    public void ShowGeneral()
    {
        generalPanel.SetActive(true);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowVideo()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowAudio()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }
    
    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    public void OnSensitivityChange()
    {
        currentSensitivity = sensitivitySliderMouse.value;
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
}
