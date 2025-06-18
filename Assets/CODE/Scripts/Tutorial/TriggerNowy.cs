using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerNowy : MonoBehaviour
{
    [Header("On Trigger Event")]
    [SerializeField] private UnityEvent todoEventIN;

    [Header("Off Trigger Event")]
    [SerializeField] private UnityEvent todoEventOut;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            todoEventIN.Invoke();
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            todoEventOut.Invoke();
        }
    }
}
