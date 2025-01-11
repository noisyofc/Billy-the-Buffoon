using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    public Slider progressBar;
    public GameObject pressAnyKeyText; // Reference to "Press any key to start" text
    public float smoothSpeed = 0.5f; // Speed for smooth slider movement

    private bool isReadyToActivate = false;
    private bool canPressKey = false; // Prevent key press until the text is fully displayed
    private float targetProgress = 0f;


//test
    private void Start()
    {
        pressAnyKeyText.SetActive(false); // Ensure the "Press any key" text is hidden initially
    }

    private void Update()
    {
        if (SceneTransitionManager.Instance.CurrentLoadingOperation != null)
        {
            // Set the target progress based on the loading operation's progress
            targetProgress = Mathf.Clamp01(SceneTransitionManager.Instance.CurrentLoadingOperation.progress / 0.9f);
        }

        // Smoothly move the slider toward the target progress
        progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, smoothSpeed * Time.deltaTime);

        // Check if the scene is ready
        if (progressBar.value >= 1f && !isReadyToActivate)
        {
            isReadyToActivate = true;
            StartCoroutine(ShowPressAnyKey());
        }

        // Detect player input to proceed, but only if the key press is allowed
        if (canPressKey && Input.anyKeyDown)
        {
            ActivateLoadedScene();
        }
    }

    private IEnumerator ShowPressAnyKey()
    {
        // Wait a short time for polish before showing the text
        yield return new WaitForSeconds(0.5f);
        pressAnyKeyText.SetActive(true); // Show the "Press any key to start" text
        //yield return new WaitForSeconds(0.5f); // Ensure delay before allowing input
        canPressKey = true; // Allow the player to press a key
    }

    private void ActivateLoadedScene()
    {
        pressAnyKeyText.SetActive(false); // Hide the text
        SceneTransitionManager.Instance.CurrentLoadingOperation.allowSceneActivation = true; // Activate the target scene
    }
}
