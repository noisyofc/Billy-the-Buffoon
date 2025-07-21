using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public bool playerHeldR;
    public GameObject[] baloons, respawnTriggers, myszors;

    void Start()
    {
        
        baloons = GameObject.FindGameObjectsWithTag("Star");
        respawnTriggers = GameObject.FindGameObjectsWithTag("TriggersTut");
        myszors = GameObject.FindGameObjectsWithTag("Myszor");
        foreach (GameObject myszor in myszors)
        {
            myszor.SetActive(false);
        }
        if (playerDiedOnce == false)
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
        if (deathScreen == null)
        {
            deathScreen = GameObject.Find("deathScreen");
        }

        if (mainUI == null)
        {
            mainUI = GameObject.Find("UI");
        }

        if (holdToRespawnSlider == null)
        {
            holdToRespawnSlider = GameObject.Find("SliderRestart").GetComponent<Slider>();
        }

        if (respawnCooldownTimer > 0f)
            respawnCooldownTimer -= Time.deltaTime;

        if (!deathScreen.activeInHierarchy && !playerDead && respawnCooldownTimer <= 0f)
        {
            HandleRespawnInput();
        }

        if (deathScreen.activeInHierarchy && playerDead)
        {
            if (Input.GetButtonDown("Restart"))
            {
                playerHeldR = true;
                Respawn();
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
                CountStars.stars = 0;
                Timer.timeElapsed = 0f;
                foreach (GameObject baloon in baloons)
                {
                    baloon.SetActive(true);
                }
                foreach (GameObject myszor in myszors)
                {
                    myszor.SetActive(false);
                }
                foreach (GameObject respawnTrigger in respawnTriggers)
                {
                    respawnTrigger.GetComponent<Collider>().enabled = true;
                }
            }
            if (Input.GetButtonDown("Restart") && SceneManager.GetActiveScene().name == "Level_0_T")
            {
                playerHeldR = false;
                Respawn();
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
        if (Input.GetButton("Restart"))
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
                CountStars.stars = 0;
                Timer.timeElapsed = 0f;
                foreach (GameObject baloon in baloons)
                {
                    baloon.SetActive(true);
                }
                foreach (GameObject myszor in myszors)
                {
                    myszor.SetActive(false);
                }
                foreach (GameObject respawnTrigger in respawnTriggers)
                {
                    respawnTrigger.GetComponent<Collider>().enabled = true;
                }
                playerHeldR = true;
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
        if (other.gameObject.CompareTag("ocean"))
        {
            Die();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject point in respawnPoints)
        {
            if (other.gameObject == point)
            {
                currentRespawnPoint = point.transform;
                point.GetComponent<Collider>().enabled = false;
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

        mainUI.SetActive(false);
        // Handle death logic here if needed (animation, etc.)

        if (currentRespawnPoint != null && respawnPoints.Length == 1)
        {
            CountStars.stars = 0;
            Timer.timeElapsed = 0f;

            foreach (GameObject baloon in baloons)
            {
                baloon.SetActive(true);
            }
            foreach (GameObject myszor in myszors)
            {
                myszor.SetActive(false);
            }
            foreach (GameObject respawnTrigger in respawnTriggers)
            {
                respawnTrigger.GetComponent<Collider>().enabled = true;
            }
        }
        playerRigidbody.isKinematic = true;
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        //Respawn();
    }

    void Respawn()
    {
        if (currentRespawnPoint != null && playerHeldR == false)
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
        else if (currentRespawnPoint != null && playerHeldR == true)
        {

            currentRespawnPoint = respawnPoints[0].transform;

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

            foreach (GameObject point in respawnPoints)
            {
                if (point.GetComponent<Collider>() != null)
                {
                    point.GetComponent<Collider>().enabled = true;
                }
            }
            playerHeldR = false;
        }
    }
}
