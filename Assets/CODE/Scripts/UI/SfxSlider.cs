using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SfxSlider : MonoBehaviour
{
    [Header("Slider SFX")]
    public Slider slider; // Slider SFX

    [Header("Lista wszystkich AudioSource od SFX na scenie")]
    public List<AudioSource> audioSources = new List<AudioSource>(); // Tu wpinamy wszystkie AudioSource od SFX

    void Start()
    {
        if (!PlayerPrefs.HasKey("SFX"))
        {
            slider.value = 1;
        }
        else
        {
            slider.value = PlayerPrefs.GetFloat("SFX");
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
        PlayerPrefs.SetFloat("SFX", slider.value);
        PlayerPrefs.Save();
    }

    public void SetDefault()
    {
        slider.value = 1;
        SaveValue();
    }
}
