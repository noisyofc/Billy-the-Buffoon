using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    private float mouseSensitivity;
    private float PadSensitivity;
    [Tooltip("Mouse sensitivity along the X-axis.")]
    public float sensX = 100f;  // Sensitivity for mouse X-axis
    [Tooltip("Mouse sensitivity along the Y-axis.")]
    public float sensY = 100f;  // Sensitivity for mouse Y-axis

    [Header("Camera References")]
    [Tooltip("The player's orientation transform used for horizontal rotation.")]
    public Transform orientation;  // Player's orientation for rotation
    [Tooltip("The camera holder object, responsible for vertical rotation.")]
    public Transform camHolder;    // Holder for the camera rotation

    // Internal rotation tracking
    private float xRotation;
    private float yRotation;

    public float yaw, pitch;

    public float yawRes1, pitchRes1, yawRes2, pitchRes2, yawRes3, pitchRes3, yawRes4, pitchRes4, yawRes5, pitchRes5, yawRes6, pitchRes6;

    public PlayerRespawnManager playerRespawnManager;

    private float tiltZ = 0f; // Add this at the top of your class

    private bool isCameraLimited = false; // Whether the camera is limited
    private float cameraLimitYawCenter = 0f;   // Center yaw (horizontal rotation)
    private float cameraLimitPitchCenter = 0f; // Center pitch (vertical rotation)
    private float cameraLimitYawHalf = 30f;    // Half-range for yaw (degrees)
    private float cameraLimitPitchHalf = 30f;  // Half-range for pitch (degrees)

    private void Start()
    {
        // Lock the cursor and hide it for first-person control
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        PadSensitivity = PlayerPrefs.GetFloat("PadSensitivity", 0.5f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerRespawnManager.respawn = true;

    }

    // Method to enable camera limits
    public void EnableCameraLimits(float yawHalf, float pitchHalf)
    {
        isCameraLimited = true;
        cameraLimitYawHalf = yawHalf;
        cameraLimitPitchHalf = pitchHalf;
        cameraLimitYawCenter = yRotation;
        cameraLimitPitchCenter = xRotation;
    }

    // Method to disable camera limits
    public void DisableCameraLimits()
    {
        isCameraLimited = false;
    }

    private void Update()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        PadSensitivity = PlayerPrefs.GetFloat("PadSensitivity", 0.5f);

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY * mouseSensitivity;
        float PadX = Input.GetAxisRaw("Pad X") * Time.deltaTime * sensX * PadSensitivity;
        float PadY = Input.GetAxisRaw("Pad Y") * Time.deltaTime * sensY * PadSensitivity;

        yRotation += mouseX;
        yRotation += PadX;
        xRotation -= mouseY;
        xRotation -= PadY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp pitch (up/down)

        if (isCameraLimited)
        {
            yRotation = Mathf.Clamp(yRotation, cameraLimitYawCenter - cameraLimitYawHalf, cameraLimitYawCenter + cameraLimitYawHalf);
            xRotation = Mathf.Clamp(xRotation, cameraLimitPitchCenter - cameraLimitPitchHalf, cameraLimitPitchCenter + cameraLimitPitchHalf);
        }

        // Apply the rotations to the camera holder and the player's orientation, including tilt
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, tiltZ);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if (playerRespawnManager.respawn == true)
        {
            xRotation = yaw;
            yRotation = pitch;
            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, tiltZ);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            if (playerRespawnManager.lastResp == "Res1")
            {
                camHolder.rotation = Quaternion.Euler(playerRespawnManager.respawnPoints[1].gameObject.transform.rotation.x, playerRespawnManager.respawnPoints[1].gameObject.transform.rotation.y, 0);
                orientation.rotation = Quaternion.Euler(0, playerRespawnManager.respawnPoints[1].gameObject.transform.rotation.y, 0);
            }
            if (playerRespawnManager.lastResp == "Res2")
            {
                camHolder.rotation = Quaternion.Euler(playerRespawnManager.respawnPoints[2].gameObject.transform.rotation.x, playerRespawnManager.respawnPoints[2].gameObject.transform.rotation.y, 0);
                orientation.rotation = Quaternion.Euler(0, playerRespawnManager.respawnPoints[2].gameObject.transform.rotation.y, 0);
            }
            if (playerRespawnManager.lastResp == "Res3")
            {
                camHolder.rotation = Quaternion.Euler(playerRespawnManager.respawnPoints[3].gameObject.transform.rotation.x, playerRespawnManager.respawnPoints[3].gameObject.transform.rotation.y, 0);
                orientation.rotation = Quaternion.Euler(0, playerRespawnManager.respawnPoints[3].gameObject.transform.rotation.y, 0);
            }
            if (playerRespawnManager.lastResp == "Res4")
            {
                camHolder.rotation = Quaternion.Euler(playerRespawnManager.respawnPoints[4].gameObject.transform.rotation.x, playerRespawnManager.respawnPoints[4].gameObject.transform.rotation.y, 0);
                orientation.rotation = Quaternion.Euler(0, playerRespawnManager.respawnPoints[4].gameObject.transform.rotation.y, 0);
            }
            if (playerRespawnManager.lastResp == "Res5")
            {
                camHolder.rotation = Quaternion.Euler(playerRespawnManager.respawnPoints[5].gameObject.transform.rotation.x, playerRespawnManager.respawnPoints[5].gameObject.transform.rotation.y, 0);
                orientation.rotation = Quaternion.Euler(0, playerRespawnManager.respawnPoints[5].gameObject.transform.rotation.y, 0);
            }
            if (playerRespawnManager.lastResp == "Res6")
            {
                camHolder.rotation = Quaternion.Euler(playerRespawnManager.respawnPoints[6].gameObject.transform.rotation.x, playerRespawnManager.respawnPoints[6].gameObject.transform.rotation.y, 0);
                orientation.rotation = Quaternion.Euler(0, playerRespawnManager.respawnPoints[6].gameObject.transform.rotation.y, 0);
            }
            playerRespawnManager.respawn = false;   
        }
    }

    /// <summary>
    /// Smoothly changes the field of view (FOV) to the specified value using DOTween.
    /// </summary>
    /// <param name="endValue">The target FOV value.</param>
    public void DoFov(float endValue)
    {
        Camera.main.DOFieldOfView(endValue, 0.25f);  // Smoothly adjust FOV to the target value over 0.25 seconds
    }

    /// <summary>
    /// Smoothly tilts the camera along the Z-axis using DOTween (currently not used).
    /// </summary>
    /// <param name="zTilt">The amount of tilt on the Z-axis.</param>
    public void DoTilt(float zTilt)
    {
        tiltZ = zTilt; // Update the tilt value
    }
}
