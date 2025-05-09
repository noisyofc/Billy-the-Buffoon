using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HoldRtoReset : MonoBehaviour
{
    public Slider countdownSlider;    // Assign in Inspector
    public float holdTime = 2f;
    public GameObject deathScreen, endScreen;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool canStartHold = true; // Prevents holding right after scene load

    void Start()
    {
        if (countdownSlider != null)
            countdownSlider.gameObject.SetActive(false);

        // If R is held down on load, wait for release before allowing holding again
        if (Input.GetKey(KeyCode.R))
            canStartHold = false;
        StartCoroutine(DisableHoldTemporarily());
    }

    void Update()
    {
        if (!canStartHold)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                canStartHold = true; // Now we can allow holding again
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && deathScreen.activeInHierarchy == false && endScreen.activeInHierarchy == false)
        {
            isHolding = true;
            if (countdownSlider != null)
                countdownSlider.gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.R))
        {
            holdTimer += Time.deltaTime;

            if (countdownSlider != null)
                countdownSlider.value = holdTimer;

            if (holdTimer >= holdTime)
            {
                RestartLevel();
            }
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            ResetHold();
        }
    }

    void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;

        if (countdownSlider != null)
        {
            countdownSlider.value = 0f;
            countdownSlider.gameObject.SetActive(false);
        }
    }

    void RestartLevel()
    {
        // Use SceneManager to reload, and this logic will run again on Start
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Disable holding functionality for 2 seconds after restart
    }

    IEnumerator DisableHoldTemporarily()
    {
        canStartHold = false; // Disable holding
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        canStartHold = true; // Re-enable holding
    }
}
