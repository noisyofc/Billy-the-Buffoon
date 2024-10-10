using System.Collections;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("The total time elapsed since the timer started.")]
    public static float timeElapsed = 0f;  // Stores the total time elapsed

    [Header("UI Components")]
    [Tooltip("Text component that will display the timer.")]
    public TextMeshProUGUI timerText;  // Reference to the UI text element that shows the timer

    public static Timer instance;  // Singleton instance for easy access from other scripts

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance of the timer exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }

    private void Update()
    {
        // Update the time elapsed based on the time since the last frame
        timeElapsed += Time.deltaTime;

        // Convert the time elapsed into minutes and seconds format
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        // Update the UI text with the formatted time
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
