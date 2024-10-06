using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountStars : MonoBehaviour
{
    [Header("Star Count")]
    [Tooltip("The current number of collected stars.")]
    public int stars = 0;  // Tracks the number of collected stars

    [Header("Star UI Elements")]
    [Tooltip("The UI objects representing collected stars.")]
    public GameObject[] starUIObjects;  // Array of UI objects to display collected stars

    public static CountStars instance;  // Singleton instance of the CountStars class

    private void Awake()
    {
        // Initialize the singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);  // Ensure there's only one instance
        }
    }

    private void Start()
    {
        // Initialize star count to 0 at the start
        stars = 0;

        // Ensure all star UI elements are inactive at the start
        foreach (GameObject starUI in starUIObjects)
        {
            starUI.SetActive(false);
        }
    }

    /// <summary>
    /// Handles star collection when the player collides with a star object.
    /// </summary>
    /// <param name="other">Collider of the other object involved in the collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object the player collided with has the "Star" tag
        if (other.CompareTag("Star"))
        {
            stars += 1;  // Increase the star count

            // Activate the corresponding UI element for the collected star
            if (stars - 1 < starUIObjects.Length)
            {
                starUIObjects[stars - 1].SetActive(true);
            }

            // Optionally, you could destroy the star object after collection
            Destroy(other.gameObject);
        }
    }
}
