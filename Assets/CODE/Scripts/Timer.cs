using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeElapsed;
    public TextMeshProUGUI timerText;

    public static Timer instance;

    private void Awake()
    {
        instance = this;
    }


    void Update()
    {
        // Update the time elapsed
        timeElapsed += Time.deltaTime;

        // Format the time elapsed as minutes and seconds
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        // Update the timer text
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
