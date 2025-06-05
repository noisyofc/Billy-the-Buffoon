using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HoldRtoReset : PlayerDeath
{
    private Slider countdownSlider;    // Assign in Inspector
    public float holdTime = 2f;
    private GameObject deathScreen, endScreen;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool canStartHold = true; // Prevents holding right after scene load

    void Start()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "SliderRestart" && obj.GetComponent<Slider>() != null)
            {
                countdownSlider = obj.GetComponent<Slider>();
            }
            else if (obj.name == "deathScreen" || obj.name == "deathScreen Desert")
            {
                deathScreen = obj;
            }
            else if (obj.name == "endScreen")
            {
                endScreen = obj;
            }
        }

        if (countdownSlider != null)
            countdownSlider.gameObject.SetActive(false);

        // If R is held down on load, wait for release before allowing holding again
        if (Input.GetButton("Restart"))
            canStartHold = false;
        StartCoroutine(DisableHoldTemporarily());
    }

    void Update()
    {
        if (countdownSlider == null || deathScreen == null || endScreen == null)
        {
            Secure();
        }

        if (!canStartHold)
        {
            if (Input.GetButton("Restart"))
            {
                canStartHold = true; // Now we can allow holding again
            }
            return;
        }

        if (Input.GetButtonDown("Restart") && deathScreen.activeInHierarchy == false && endScreen.activeInHierarchy == false)
        {
            isHolding = true;
            if (countdownSlider != null)
                countdownSlider.gameObject.SetActive(true);
        }

        if (Input.GetButton("Restart"))
        {
            holdTimer += Time.deltaTime;

            if (countdownSlider != null)
                countdownSlider.value = holdTimer;

            if (holdTimer >= holdTime)
            {
                RestartLevel();
            }
        }

        if (Input.GetButtonUp("Restart"))
        {
            ResetHold();
        }
    }

    public void Secure()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "SliderRestart" && obj.GetComponent<Slider>() != null)
            {
                countdownSlider = obj.GetComponent<Slider>();
            }
            else if (obj.name == "deathScreen" || obj.name == "deathScreen Desert")
            {
                deathScreen = obj;
            }
            else if (obj.name == "endScreen")
            {
                endScreen = obj;
            }
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        RespawnPlayer();
        // Disable holding functionality for 2 seconds after restart
    }

    IEnumerator DisableHoldTemporarily()
    {
        canStartHold = false; // Disable holding
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        canStartHold = true; // Re-enable holding
    }
}
