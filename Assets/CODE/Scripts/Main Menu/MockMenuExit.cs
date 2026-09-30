using UnityEngine;

public class MockMenuExit : MonoBehaviour
{
    public Material[] material;
    public GameObject panelSelectLevel, panelOptions;
    public float flickerInterval = 0.2f;

    private Renderer[] _neonRenderers;
    private float timer;
    private bool isOnState = true;
    private bool isMouseOver;

    void Start()
    {
#if UNITY_WEBGL
        gameObject.SetActive(false);
        return;
#endif
        _neonRenderers = MenuNeonFlicker.CacheChildRenderers(transform);
        MenuNeonFlicker.SetAll(_neonRenderers, material[0]);
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
        if (!panelSelectLevel.activeSelf && !panelOptions.activeSelf)
        {
            OptionsManager.StopVibration();
            Application.Quit();
        }
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
}
