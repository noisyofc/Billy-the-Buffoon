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

    [SerializeField] public GameObject[] respawnPoints;

    private void Start()
    {
        RespawnPlayer(); // Set initial spawn
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ocean"))
        {
            EndScreen.endLevel = true;
            RespawnPlayer();
            PlayerMovementAdvanced.Paused = true;
            deathScreen.SetActive(true);
            playerDead = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CountStars.stars = 0;
            Timer.timeElapsed = 0f;
            mainUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject point in respawnPoints)
        {
            if (other.gameObject == point)
            {
                // Update the respawn position and rotation
                respawnPosition = point.transform.position;
                respawnRotation = point.transform.eulerAngles;
                Debug.Log("Updated respawn point to: " + point.name);
                break;
            }
        }
    }

    public void RespawnPlayer()
    {
        transform.position = respawnPosition;
        transform.rotation = Quaternion.Euler(respawnRotation);
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }
}
