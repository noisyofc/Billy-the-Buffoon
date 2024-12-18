using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    [Header("Slider Music")]
    public Slider slider; // Slider Music

    [Header("Lista wszystkich AudioSource od Muzyki na scenie")]
    public List<AudioSource> audioSources = new List<AudioSource>(); // Tu wpinamy wszystkie AudioSource od Muzyki

    void Start()
    {
        if (!PlayerPrefs.HasKey("Music"))
        {
            slider.value = 0.5f;
        }
        else
        {
            slider.value = PlayerPrefs.GetFloat("Music");
        }
    }

    void Update()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = slider.value;
        }
    }

    public void SaveValue()
    {
        PlayerPrefs.SetFloat("Music", slider.value);
        PlayerPrefs.Save();
    }

    public void SetDefault()
    {
        slider.value = 0.5f;
        SaveValue();
    }
}
