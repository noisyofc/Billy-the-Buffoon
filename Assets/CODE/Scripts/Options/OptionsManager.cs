using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public GameObject generalPanel;
    public GameObject videoPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;

    public Slider sensitivitySliderMouse;
    public Slider sensitivitySliderPad;
    public float defaultSensitivity = 1.0f;
    public float defaultSensitivityPad = 0.5f;
    private float currentSensitivity;
    private float currentSensitivityPad;

    public Image general;
    public Image audio;
    public Image video;
    public Image controls;

    public void Start()
    {
        LoadSensitivity();
        sensitivitySliderMouse.value = currentSensitivity;
        sensitivitySliderPad.value = currentSensitivityPad;
        sensitivitySliderMouse.onValueChanged.AddListener(delegate { OnSensitivityChange(); });
        sensitivitySliderPad.onValueChanged.AddListener(delegate { OnSensitivityChangePad(); });
    }

    public void ShowGeneral()
    {
        generalPanel.SetActive(true);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);

        ChangeColor("#8C8C8C", general);
        ChangeColor("#FFFFFF", audio);
        ChangeColor("#FFFFFF", video);
        ChangeColor("#FFFFFF", controls);
    }

    public void ShowVideo()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);

        ChangeColor("#FFFFFF", general);
        ChangeColor("#FFFFFF", audio);
        ChangeColor("#8C8C8C", video);
        ChangeColor("#FFFFFF", controls);
    }

    public void ShowAudio()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);

        ChangeColor("#FFFFFF", general);
        ChangeColor("#8C8C8C", audio);
        ChangeColor("#FFFFFF", video);
        ChangeColor("#FFFFFF", controls);
    }
    
    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);

        ChangeColor("#FFFFFF", general);
        ChangeColor("#FFFFFF", audio);
        ChangeColor("#FFFFFF", video);
        ChangeColor("#8C8C8C", controls);
    }

    public void OnSensitivityChange()
    {
        currentSensitivity = sensitivitySliderMouse.value;
        SaveSensitivity();
    }

    public void OnSensitivityChangePad()
    {
        currentSensitivityPad = sensitivitySliderPad.value;
        SaveSensitivityPad();
    }

    public void SaveSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", currentSensitivity);
        PlayerPrefs.Save();
        Debug.Log("Sensitivity Saved: " + currentSensitivity);
    }

    public void SaveSensitivityPad()
    {
        PlayerPrefs.SetFloat("PadSensitivity", currentSensitivityPad);
        PlayerPrefs.Save();
        Debug.Log("Sensitivity Pad Saved: " + currentSensitivityPad);
    }

    public void LoadSensitivity()
    {
        currentSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);
        currentSensitivityPad = PlayerPrefs.GetFloat("PadSensitivity", defaultSensitivity);
        Debug.Log("Sensitivity Loaded: " + currentSensitivity);
    }

    public void SetSenseToDefault()
    {
        currentSensitivity = defaultSensitivity;
        currentSensitivityPad = defaultSensitivityPad;

        sensitivitySliderMouse.value = defaultSensitivity;
        sensitivitySliderPad.value = defaultSensitivityPad;

        SaveSensitivity();
        SaveSensitivityPad();
    }

    public void ChangeColor(string hexColor, Image buttonImage)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            buttonImage.color = newColor;
            Debug.Log($"Kolor zmieniony na: {newColor}");
        }
        else
        {
            Debug.LogError("Nieprawid³owy kod koloru: " + hexColor);
        }
    }
}
