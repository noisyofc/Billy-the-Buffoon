using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    private float mouseSensitivity;
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

    private void Start()
    {
        // Lock the cursor and hide it for first-person control
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    private void Update()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        // Get mouse input for both X and Y axes
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY * mouseSensitivity;

        // Adjust the yaw (Y rotation) based on mouse X movement
        yRotation += mouseX;

        // Adjust the pitch (X rotation) based on mouse Y movement and clamp it between -90 and 90 degrees
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotations to the camera holder and the player's orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
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
        // Smoothly rotate the camera along the Z-axis (disabled for now, uncomment when needed)
        // camHolder.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
