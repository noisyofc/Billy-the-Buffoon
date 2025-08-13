using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringOnTramp : MonoBehaviour
{
    public AudioSource audioSource;
    public static AudioSource audioSourceStatic;

    public void Update()
    {
        if (audioSourceStatic == null)
        {
            audioSourceStatic = audioSource;
        }
    }

    public static void PlaySound()
    {
        audioSourceStatic.Play();
    }
}
