using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTagOfWall : MonoBehaviour
{
    [Header("Tag Settings")]
    [Tooltip("The tag that should be assigned when the wall is glued.")]
    public string gluedTag = "glued";
    [Tooltip("The tag that should be restored after the countdown.")]
    public string defaultTag = "Wall";

    [Header("Countdown Settings")]
    [Tooltip("Time to wait before resetting the wall tag.")]
    public float resetTime = 1f;  // Time before resetting tag

    private void Start()
    {
        // Set the initial tag of the game object to "Wall"
        gameObject.tag = defaultTag;
    }

    private void Update()
    {
        // Check if the wall's tag has changed to "glued"
        if (gameObject.tag == gluedTag)
        {
            // Start the countdown coroutine to revert the tag back to "Wall"
            StartCoroutine(ResetTagAfterDelay());
        }
    }

    /// <summary>
    /// Resets the wall's tag back to its default value after a delay.
    /// </summary>
    private IEnumerator ResetTagAfterDelay()
    {
        // Wait for the specified reset time
        yield return new WaitForSeconds(resetTime);

        // Reset the tag back to "Wall"
        gameObject.tag = defaultTag;

        // Disable the first two child objects (if present)
        if (gameObject.transform.childCount >= 2)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
