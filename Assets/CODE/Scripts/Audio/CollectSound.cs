using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSound : MonoBehaviour
{
    public AudioSource collect;

    public void Collected()
    {
        collect.Play();
    }
}
