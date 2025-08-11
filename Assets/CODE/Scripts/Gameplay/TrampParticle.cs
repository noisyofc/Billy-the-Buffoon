using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampParticle : MonoBehaviour
{
    public GameObject particle;

    private void OnTriggerEnter(Collider other)
    {
        particle.SetActive(true);
    }
}
