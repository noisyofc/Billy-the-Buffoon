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

    public Image generalButton;
    public Image audioButton;
    public Image videoButton;
    public Image controlsButton;

    public Sprite general;
    public Sprite audio;
    public Sprite video;
    public Sprite controls;

    public Sprite generalPressed;
    public Sprite audioPressed;
    public Sprite videoPressed;
    public Sprite controlsPressed;

    public Image toggle1280x720Button;
    public Image toggle1920x1080Button;
    public Image toggle3840x2160Button;

    public Sprite toggle1280x720;
    public Sprite toggle1920x1080;
    public Sprite toggle3840x2160;

    public Sprite toggle1280x720Pressed;
    public Sprite toggle1920x1080Pressed;
    public Sprite toggle3840x2160Pressed;

    public void Start()
    {
        LoadSensitivity();
        sensitivitySliderMouse.value = currentSensitivity;
        sensitivitySliderPad.value = currentSensitivityPad;
        sensitivitySliderMouse.onValueChanged.AddListener(delegate { OnSensitivityChange(); });
        sensitivitySliderPad.onValueChanged.AddListener(delegate { OnSensitivityChangePad(); });
        CheckRes();
    }

    public void ShowGeneral()
    {
        generalPanel.SetActive(true);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);

        generalButton.sprite = generalPressed;
        audioButton.sprite = audio;
        videoButton.sprite = video;
        controlsButton.sprite = controls;
    }

    public void ShowVideo()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);

        generalButton.sprite = general;
        audioButton.sprite = audio;
        videoButton.sprite = videoPressed;
        controlsButton.sprite = controls;
    }

    public void ShowAudio()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);

        generalButton.sprite = general;
        audioButton.sprite = audioPressed;
        videoButton.sprite = video;
        controlsButton.sprite = controls;
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);

        generalButton.sprite = general;
        audioButton.sprite = audio;
        videoButton.sprite = video;
        controlsButton.sprite = controlsPressed;
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

    public void CheckRes()
    {
        if (!PlayerPrefs.HasKey("Res"))
        {
            SetResolution1920x1080();
        }

        if (PlayerPrefs.GetInt("Res") == 1)
        {
            SetResolution1280x720();
        }
        else if (PlayerPrefs.GetInt("Res") == 2)
        {
            SetResolution1920x1080();
        }
        else if (PlayerPrefs.GetInt("Res") == 3)
        {
            SetResolution3840x2160();
        }
    }

    public void SetResolution1280x720()
    {
        Screen.SetResolution(1280, 720, Screen.fullScreen);
        PlayerPrefs.SetInt("Res", 1);
        PlayerPrefs.Save();
        toggle1280x720Button.sprite = toggle1280x720Pressed;
        toggle1920x1080Button.sprite = toggle1920x1080;
        toggle3840x2160Button.sprite = toggle3840x2160;
        Debug.Log("Ustawiono rozdzielczoœæ 1280x720");
    }

    public void SetResolution1920x1080()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
        PlayerPrefs.SetInt("Res", 2);
        PlayerPrefs.Save();
        toggle1280x720Button.sprite = toggle1280x720;
        toggle1920x1080Button.sprite = toggle1920x1080Pressed;
        toggle3840x2160Button.sprite = toggle3840x2160;
        Debug.Log("Ustawiono rozdzielczoœæ 1920x1080");
    }

    public void SetResolution3840x2160()
    {
        Screen.SetResolution(3840, 2160, Screen.fullScreen);
        PlayerPrefs.SetInt("Res", 3);
        PlayerPrefs.Save();
        toggle1280x720Button.sprite = toggle1280x720;
        toggle1920x1080Button.sprite = toggle1920x1080;
        toggle3840x2160Button.sprite = toggle3840x2160Pressed;
        Debug.Log("Ustawiono rozdzielczoœæ 1920x1080");
    }

    public void SetResDefault()
    {
        SetResolution1920x1080();
    }
}
