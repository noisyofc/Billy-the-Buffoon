using System.Collections;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("The target position that the camera will follow.")]
    public Transform cameraPosition;  // The transform the camera will follow

    private void Update()
    {
        // Update the camera's position to match the target's position every frame
        transform.position = cameraPosition.position;
    }
}
