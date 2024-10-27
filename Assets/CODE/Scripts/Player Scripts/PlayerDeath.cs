using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("The player's respawn position.")]
    public Vector3 respawnPosition = new Vector3(-8.5f, 16f, -5.5f);  // Default respawn position
    [Tooltip("The player's respawn rotation.")]
    public Vector3 respawnRotation = new Vector3(0f, 0f, 0f);         // Default respawn rotation

    public Rigidbody playerRigidbody;
    public GameObject deathScreen;
    public static bool playerDead = false;
    public GameObject mainUI;

    private void Start()
    {
        // Set the player's initial position and rotation at the start of the game
        RespawnPlayer();
    }

    /// <summary>
    /// Handles the player's collision detection. If the player collides with the ocean, respawn them.
    /// </summary>
    /// <param name="collision">Collision data.</param>
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collided with the ocean
        if (collision.gameObject.CompareTag("ocean"))
        {
            RespawnPlayer();  // Respawn the player if they fall into the ocean
            PlayerMovementAdvanced.Paused = true;
            deathScreen.SetActive(true);
            playerDead = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            CountStars.stars = 0;
            Timer.timeElapsed = 0f;
            mainUI.SetActive(false);
        }
        
    }

    /// <summary>
    /// Respawns the player at the specified position and rotation.
    /// </summary>
    private void RespawnPlayer()
    {
        // Set the player's position and rotation to the respawn values
        transform.position = respawnPosition;
        transform.rotation = Quaternion.Euler(respawnRotation);
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }
}
