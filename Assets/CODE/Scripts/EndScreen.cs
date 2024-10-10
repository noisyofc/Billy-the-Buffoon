using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Text element displaying the number of collected stars.")]
    public TextMeshProUGUI starsEnd;  // UI element for showing stars at the end screen
    [Tooltip("Text element displaying the total time.")]
    public TextMeshProUGUI timeEnd;   // UI element for showing time at the end screen
    [Tooltip("Image representing the end screen background.")]
    public Image endScreen;           // Image component for the end screen background
    [Tooltip("The main canvas that will be disabled when the end screen shows.")]
    public GameObject canvas;         // Canvas to hide when end screen is shown

    [Header("Game Data")]
    [Tooltip("Number of stars collected by the player.")]
    public int stars = 0;  // Number of stars collected
    [Tooltip("Total time taken by the player.")]
    public float time = 0f;  // Total time spent

    private void Update()
    {
        // Update the stars display
        starsEnd.text = stars.ToString();

        // Format the time to MM:SS and update the time display
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeEnd.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// When the player collides with the end zone, trigger the end screen.
    /// </summary>
    /// <param name="other">Collider of the object that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Pause the game
            Time.timeScale = 0;

            // Unlock and make the cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Hide the gameplay canvas and show the end screen
            canvas.SetActive(false);
            endScreen.gameObject.SetActive(true);

            // Get stars collected and time from other game components
            stars = CountStars.stars;
            time = Timer.timeElapsed;
        }
    }
}
