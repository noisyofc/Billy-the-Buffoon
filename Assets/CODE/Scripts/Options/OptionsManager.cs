using UnityEngine;
using UnityEngine.InputSystem;
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

    public Toggle vibrationToggle;

    private static bool isVibrating = false;
    private static float vibrationStopTime = 0f;

    public static bool enableVibration;

    public GameObject[] videoSettingsITCHIO;
    public GameObject textITCHIO;

    public GameObject toggleVib;

    public Toggle fullscreenToggle;

    public void Start()
    {
        textITCHIO.SetActive(false);
        //Screen.SetResolution(1920, 1080, Screen.fullScreen);
        LoadSensitivity();
        sensitivitySliderMouse.value = currentSensitivity;
        sensitivitySliderPad.value = currentSensitivityPad;
        sensitivitySliderMouse.onValueChanged.AddListener(delegate { OnSensitivityChange(); });
        sensitivitySliderPad.onValueChanged.AddListener(delegate { OnSensitivityChangePad(); });

#if !UNITY_WEBGL
        CheckRes();
        CheckFullScreen();
        CheckVibration();
#endif

#if UNITY_WEBGL
        foreach (GameObject item in videoSettingsITCHIO)
        {
            item.SetActive(false);
        }

        toggleVib.SetActive(false);

        textITCHIO.SetActive(true);
#endif
#if !UNITY_WEBGL
        foreach (GameObject item in videoSettingsITCHIO)
        {
            item.SetActive(true);
        }

        toggleVib.SetActive(true);

        textITCHIO.SetActive(false);
#endif

    }

    public void Update()
    {
        if (isVibrating && Time.unscaledTime >= vibrationStopTime)
        {
            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0f, 0f);
            }

            isVibrating = false;
        }
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

        enableVibration = true;
        PlayerPrefs.SetInt("Vib", 1);
        vibrationToggle.isOn = true;
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
        Debug.Log("Ustawiono rozdzielczo�� 1280x720");
    }

    public void SetResolution1920x1080()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
        PlayerPrefs.SetInt("Res", 2);
        PlayerPrefs.Save();
        toggle1280x720Button.sprite = toggle1280x720;
        toggle1920x1080Button.sprite = toggle1920x1080Pressed;
        toggle3840x2160Button.sprite = toggle3840x2160;
        Debug.Log("Ustawiono rozdzielczo�� 1920x1080");
    }

    public void SetResolution3840x2160()
    {
        Screen.SetResolution(3840, 2160, Screen.fullScreen);
        PlayerPrefs.SetInt("Res", 3);
        PlayerPrefs.Save();
        toggle1280x720Button.sprite = toggle1280x720;
        toggle1920x1080Button.sprite = toggle1920x1080;
        toggle3840x2160Button.sprite = toggle3840x2160Pressed;
        Debug.Log("Ustawiono rozdzielczo�� 3840x2160");
    }

    private void SetScreenMode(int resIndex, bool isFullscreen)
    {
        int width = 1920;
        int height = 1080;

        switch (resIndex)
        {
            case 1:
                width = 1280;
                height = 720;
                break;
            case 2:
                width = 1920;
                height = 1080;
                break;
            case 3:
                width = 3840;
                height = 2160;
                break;
            default:
                Debug.LogWarning("Nieznana wartość 'Res' w PlayerPrefs!");
                break;
        }

        var mode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(width, height, mode);
    }

    public void SetResDefault()
    {
        SetResolution1920x1080();

        fullscreenToggle.isOn = true;
        int resIndex = PlayerPrefs.GetInt("Res", 0);
        SetScreenMode(resIndex, true);
        PlayerPrefs.SetInt("FullScreen", 1);
        PlayerPrefs.Save();
    }

    public void ToggleVibration()
    {
        enableVibration = vibrationToggle.isOn;
        PlayerPrefs.SetInt("Vib", enableVibration ? 1 : 0);

        Debug.Log("VIB " + enableVibration);
    }

    public void CheckVibration()
    {
        if (!PlayerPrefs.HasKey("Vib"))
        {
            enableVibration = true;
            PlayerPrefs.SetInt("Vib", 1);
            vibrationToggle.isOn = enableVibration;
        }
        else
        {
            if (PlayerPrefs.GetInt("Vib") == 1)
            {
                enableVibration = true;
                vibrationToggle.isOn = enableVibration;
            }
            else if (PlayerPrefs.GetInt("Vib") == 0)
            {
                enableVibration = false;
                vibrationToggle.isOn = enableVibration;
            }
        }
    }

    public static void TriggerVibration()
    {
        if (!enableVibration) return;

        float lowFrequency = 0.5f;
        float highFrequency = 0.5f;

        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            isVibrating = true;
            vibrationStopTime = Time.unscaledTime + 1f;
        }
        else
        {
            Debug.Log("Nie wykryto gamepada.");
        }
    }

    public static void StopVibration()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }

    public void CheckFullScreen()
    {
        int resIndex = PlayerPrefs.GetInt("Res", 0);
        bool isFullscreen = PlayerPrefs.GetInt("FullScreen", 0) == 1;

        if (!PlayerPrefs.HasKey("FullScreen"))
        {
            isFullscreen = true;
            PlayerPrefs.SetInt("FullScreen", 1);
            PlayerPrefs.Save();
        }

        SetScreenMode(resIndex, isFullscreen);

        fullscreenToggle.onValueChanged.RemoveAllListeners();
        fullscreenToggle.isOn = isFullscreen;
        fullscreenToggle.onValueChanged.AddListener(_ => SetFullscreen());
    }

    public void SetFullscreen()
    {
        bool isFullscreen = fullscreenToggle.isOn;

        int resIndex = PlayerPrefs.GetInt("Res", 2);

        SetScreenMode(resIndex, isFullscreen);

        PlayerPrefs.SetInt("FullScreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetScreenMode(bool isFullscreen)
    {
        var width = Screen.width;
        var height = Screen.height;
        var mode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(width, height, mode);
    }
}
