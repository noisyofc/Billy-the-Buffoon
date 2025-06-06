using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawnManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    public GameObject[] respawnPoints; // Set this in Inspector
    public Slider holdToRespawnSlider; // Reference to the UI slider

    private Transform currentRespawnPoint;
    private float holdTime = 0f;
    public float requiredHoldTime = 2f;
    public GameObject deathScreen, mainUI;
    public static bool playerDead = false;
    private bool isHoldingR = false;
    public Rigidbody playerRigidbody;

    public bool playerDiedOnce = false;

    private float respawnCooldown = 2f;
    private float respawnCooldownTimer = 0f;

    void Start()
    {
        if(playerDiedOnce == false)
        {
            playerDiedOnce = true;
            // Set the player's initial position and rotation at the start of the game
            Respawn();
        }
        // Start at the first respawn point by default
        if (respawnPoints.Length > 0)
            currentRespawnPoint = respawnPoints[0].transform;

        if (holdToRespawnSlider != null)
            holdToRespawnSlider.gameObject.SetActive(false);
    }
    void Update()
    {
        if (respawnCooldownTimer > 0f)
            respawnCooldownTimer -= Time.deltaTime;

        if (!deathScreen.activeInHierarchy && !playerDead && respawnCooldownTimer <= 0f)
        {
            HandleRespawnInput();
        }

        if (deathScreen.activeInHierarchy && playerDead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                EndScreen.endLevel = false;
                PlayerMovementAdvanced.Paused = false;
                deathScreen.SetActive(false);
                playerDead = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mainUI.SetActive(true);
                holdToRespawnSlider.gameObject.SetActive(false);
                isHoldingR = false;
                holdTime = 0f;
            }
        }
    }

    void HandleRespawnInput()
    {
        if (Input.GetKey(KeyCode.R))
        {
            isHoldingR = true;
            holdTime += Time.deltaTime;

            if (holdToRespawnSlider != null)
            {
                holdToRespawnSlider.gameObject.SetActive(true);
                holdToRespawnSlider.value = holdTime / requiredHoldTime;
            }

            if (holdTime >= requiredHoldTime)
            {
                Respawn();
                ResetHold();
            }
        }
        else if (isHoldingR)
        {
            ResetHold();
        }
    }

    void ResetHold()
    {
        isHoldingR = false;
        holdTime = 0f;

        if (holdToRespawnSlider != null)
        {
            holdToRespawnSlider.value = 0;
            holdToRespawnSlider.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Check for Ocean collision
        if (other.gameObject.CompareTag("ocean"))
        {
            Die();
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        // Check for respawn point triggers
        foreach (GameObject point in respawnPoints)
        {
            if (other.gameObject == point)
            {
                currentRespawnPoint = point.transform;
                break;
            }
        }
    }

    void Die()
    {
        EndScreen.endLevel = true;
        PlayerMovementAdvanced.Paused = true;
        deathScreen.SetActive(true);
        playerDead = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Time.timeScale = 0;
        CountStars.stars = 0;
        Timer.timeElapsed = 0f;
        mainUI.SetActive(false);
        // Handle death logic here if needed (animation, etc.)
        Respawn();
    }

    void Respawn()
    {
        if (currentRespawnPoint != null)
        {
            respawnCooldownTimer = respawnCooldown;
            // Temporarily disable physics
            playerRigidbody.isKinematic = true;

            // Move player to respawn point
            playerRigidbody.position = currentRespawnPoint.position;
            playerRigidbody.rotation = currentRespawnPoint.rotation;
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;

            // Re-enable physics
            playerRigidbody.isKinematic = false;
            // Optionally reset velocity, animations, etc.
        }
    }
}
