using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MockMenuSelectLevel : MonoBehaviour
{
    public GameObject panelSelectLevel, panelOptions;
    public Material[] material;
    public float flickerInterval = 0.2f;

    public GameObject OptionsButton;
    public GameObject LevelButton;
    public GameObject QuitButton;
    public GameObject StartButton;
    public Camera mainCamera;

    public static bool isMouseOver;

    private Renderer[] _neonRenderers;
    private float timer;
    private bool isOnState = true;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    private GameObject credits;

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
        _neonRenderers = MenuNeonFlicker.CacheChildRenderers(transform);
        MenuNeonFlicker.SetAll(_neonRenderers, material[0]);

        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);
        credits = GameObject.FindGameObjectWithTag("Credits");
        CacheMenuButtons();
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

    void Update()
    {
        if (isMouseOver || _neonRenderers == null)
            return;

        timer += Time.deltaTime;
        if (timer < flickerInterval)
            return;

        isOnState = !isOnState;
        timer = 0f;
        MenuNeonFlicker.ApplyAlternating(_neonRenderers, material[1], material[0], isOnState);
    }

    private void OnMouseUpAsButton()
    {
        if (panelSelectLevel.activeSelf || panelOptions.activeSelf)
            return;

        panelSelectLevel.SetActive(true);
        SetMenuButtonsInteractable(false);
        EnableBlur();
        if (credits != null)
            credits.SetActive(false);
    }

    private void SetMenuButtonsInteractable(bool enabled)
    {
        if (_levelMenu != null) _levelMenu.enabled = enabled;
        if (_levelCollider != null) _levelCollider.enabled = enabled;
        if (_quitMenu != null) _quitMenu.enabled = enabled;
        if (_quitCollider != null) _quitCollider.enabled = enabled;
        if (_startMenu != null) _startMenu.enabled = enabled;
        if (_startCollider != null) _startCollider.enabled = enabled;
        if (_optionsMenu != null) _optionsMenu.enabled = enabled;
        if (_optionsCollider != null) _optionsCollider.enabled = enabled;
    }

    private void OnMouseEnter()
    {
        if (panelSelectLevel.activeSelf || panelOptions.activeSelf)
            return;

        isMouseOver = true;
        MenuNeonFlicker.SetAll(_neonRenderers, material[1]);
    }

    private void OnMouseExit()
    {
        if (!panelSelectLevel.activeSelf && !panelOptions.activeSelf)
            isMouseOver = false;
    }

    public void EnableBlur()
    {
        if (depthOfField != null)
            depthOfField.active = true;
    }

    public void DisableBlur()
    {
        if (depthOfField != null)
            depthOfField.active = false;
    }
}
