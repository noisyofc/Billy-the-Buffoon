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
    public float xRotation;
    public float yRotation;

    public float yaw, pitch;

    public float yawRes1, pitchRes1, yawRes2, pitchRes2, yawRes3, pitchRes3, yawRes4, pitchRes4, yawRes5, pitchRes5, yawRes6, pitchRes6;

    public PlayerRespawnManager playerRespawnManager;

    private float tiltZ = 0f; // Add this at the top of your class

    public bool cameraLocked = false;
    public Vector2 lockedRotation;
    [Header("Wallrun Camera Limits")]
    public float maxYawOffset = 10f;   // maksymalny ruch w poziomie
    public float maxPitchOffset = 5f;  // maksymalny ruch w pionie
    private Vector2 wallrunLockedRotation; // kierunek centralny podczas wallrunu

    private void Start()
    {
        // Lock the cursor and hide it for first-person control
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        PadSensitivity = PlayerPrefs.GetFloat("PadSensitivity", 0.5f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerRespawnManager.respawn = true;

    }

    private void Update()
    {
        if (cameraLocked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX * mouseSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY * mouseSensitivity;

            yRotation += mouseX;
            xRotation -= mouseY;

            // Ograniczenie ruchu wzglêdem centralnego punktu
            xRotation = Mathf.Clamp(xRotation, wallrunLockedRotation.x - maxPitchOffset, wallrunLockedRotation.x + maxPitchOffset);
            yRotation = Mathf.Clamp(yRotation, wallrunLockedRotation.y - maxYawOffset, wallrunLockedRotation.y + maxYawOffset);

            camHolder.localRotation = Quaternion.Euler(xRotation, yRotation, tiltZ);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            return;
        }
        else
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
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Apply the rotations to the camera holder and the player's orientation, including tilt
            //   camHolder.rotation = Quaternion.Euler(xRotation, yRotation, tiltZ);
            camHolder.localRotation = Quaternion.Euler(xRotation, yRotation, tiltZ);
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
        tiltZ = zTilt;
        // Optionally keep the DOTween for smoothness, but always update tiltZ
      //  camHolder.DOLocalRotate(new Vector3(xRotation, yRotation, tiltZ), 0.25f);
    }

    public void LockCamera(Vector2 centerRotation)
    {
        wallrunLockedRotation = centerRotation;
        cameraLocked = true;
    }

    public void UnlockCamera()
    {
        cameraLocked = false;
    }
}
