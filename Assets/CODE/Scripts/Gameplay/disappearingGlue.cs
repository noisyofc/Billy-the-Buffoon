using System.Collections;
using UnityEngine;

public class DisappearingGlue : MonoBehaviour
{
    [Header("Disappearance Settings")]
    [Tooltip("Time (in seconds) before the glue object disappears.")]
    public float disappearanceTime = 2f;  // Time to wait before the object is destroyed

    private void Start()
    {
        // Start the countdown for disappearing
        StartCoroutine(CountdownToDisappear());
    }

    /// <summary>
    /// Coroutine that waits for the specified disappearance time before destroying the object.
    /// </summary>
    IEnumerator CountdownToDisappear()
    {
        // Wait for the specified time before destroying the object
        yield return new WaitForSeconds(disappearanceTime);

        // Destroy the game object after the time has passed
        Destroy(gameObject);
    }
}
