using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BackButton : MonoBehaviour
{
    public GameObject OptionsButton;
    public GameObject LevelButton;
    public GameObject QuitButton;
    public GameObject StartButton;
    public GameObject panelOptions;
    public Camera mainCamera;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    private GameObject credits;
    private Canvas _optionsCanvas;

    private MockMenuSelectLevel _levelMenu;
    private Collider _levelCollider;
    private MockMenuExit _quitMenu;
    private Collider _quitCollider;
    private MockMenuStartGame _startMenu;
    private Collider _startCollider;
    private MockMenuOptions _optionsMenu;
    private Collider _optionsCollider;

    void Start()
    {
        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);
        credits = GameObject.FindGameObjectWithTag("Credits");

        if (panelOptions != null)
            _optionsCanvas = panelOptions.GetComponent<Canvas>();

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

    public void goBack()
    {
        if (_optionsCanvas != null)
            _optionsCanvas.enabled = false;

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
}
