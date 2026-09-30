using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HoldRtoReset : MonoBehaviour
{
    [Header("Refs (assign in Inspector — works with inactive UI)")]
    [SerializeField] private Slider countdownSlider;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject endScreen;

    public float holdTime = 2f;

    private float holdTimer;
    private bool canStartHold = true;

    void Awake()
    {
        ResolveRefsOnce();
    }

    void Start()
    {
        if (countdownSlider != null)
            countdownSlider.gameObject.SetActive(false);

        if (GameInput.RestartHeld)
            canStartHold = false;

        StartCoroutine(DisableHoldTemporarily());
    }

    /// <summary>
    /// One-time lookup for inactive UI objects. Never called from Update.
    /// </summary>
    void ResolveRefsOnce()
    {
        if (countdownSlider != null && deathScreen != null && endScreen != null)
            return;

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (!obj.scene.IsValid())
                continue;

            if (countdownSlider == null && obj.name == "SliderRestart")
            {
                Slider slider = obj.GetComponent<Slider>();
                if (slider != null)
                    countdownSlider = slider;
            }
            else if (deathScreen == null && (obj.name == "deathScreen" || obj.name == "deathScreen Desert"))
            {
                deathScreen = obj;
            }
            else if (endScreen == null && obj.name == "endScreen")
            {
                endScreen = obj;
            }
        }
    }

    void Update()
    {
        if (countdownSlider == null || deathScreen == null || endScreen == null)
            return;

        if (!canStartHold)
        {
            if (GameInput.RestartHeld)
                canStartHold = true;
            return;
        }

        if (GameInput.RestartPressedThisFrame && !deathScreen.activeInHierarchy && !endScreen.activeInHierarchy)
        {
            countdownSlider.gameObject.SetActive(true);
        }

        if (GameInput.RestartHeld)
        {
            holdTimer += Time.deltaTime;
            countdownSlider.value = holdTimer;

            if (holdTimer >= holdTime)
                RestartLevel();
        }

        if (GameInput.RestartReleasedThisFrame)
            ResetHold();
    }

    void ResetHold()
    {
        holdTimer = 0f;

        if (countdownSlider == null)
            return;

        countdownSlider.value = 0f;
        countdownSlider.gameObject.SetActive(false);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator DisableHoldTemporarily()
    {
        canStartHold = false;
        yield return new WaitForSeconds(2f);
        canStartHold = true;
    }
}
