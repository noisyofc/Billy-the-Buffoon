using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrowingTutorial : MonoBehaviour
{
    public int choice = 0;
    public GameObject[] objectsToThrow;

    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    public int totalThrows;
    public float throwCooldown;

    private LayerMask platformLayer;

    public KeyCode trampKey = KeyCode.Mouse0;
    public KeyCode bananaKey = KeyCode.Mouse2;
    public KeyCode glueKey = KeyCode.Mouse1;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start()
    {

        platformLayer = LayerMask.GetMask("whatIsGround");

        Application.targetFrameRate = 60;  // Lock the frame rate to 60 FPS
        readyToThrow = true;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(trampKey) && readyToThrow && totalThrows > 0)
        {
            Throw(0);
        }
        if (Input.GetKeyDown(bananaKey) && readyToThrow && totalThrows > 0)
        {
            Throw(1);
        }
        if (Input.GetKeyDown(glueKey) && readyToThrow && totalThrows > 0)
        {
            Throw(2);
        }
    }

    private void Throw(int choice)
    {
        readyToThrow = false;

        // Select the correct object to throw
        objectToThrow = objectsToThrow[choice];

        // Calculate the spawn position and ensure it's above the ground
        Vector3 spawnPosition = attackPoint.position;
        RaycastHit groundHit;

        // Perform a raycast downwards from the spawn position to find the ground
        if (Physics.Raycast(spawnPosition, Vector3.down, out groundHit, Mathf.Infinity, platformLayer))
        {
            // Set spawn position slightly above the ground
            spawnPosition = groundHit.point + Vector3.up * 0.5f;  // Adjust "0.5f" to control how far above the ground to spawn
        }

        // Instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        

        // Get Rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.interpolation = RigidbodyInterpolation.Interpolate;
        projectileRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Ignore collision with the player
        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider playerCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(projectileCollider, playerCollider);

        // Calculate direction - use Raycast for more accurate aiming
        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            // Adjust direction towards hit point
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        else
        {
            // Fallback to camera's forward direction
            forceDirection = cam.transform.forward;
        }

        // Add force - use Vector3.up for consistent upward direction
        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;

        // Apply the calculated force
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        // Reduce total throws and reset throw cooldown
        totalThrows--;
        Invoke(nameof(ResetThrow), throwCooldown);

        // Debug visualization
        Debug.DrawRay(attackPoint.position, forceDirection * 10, Color.red, 2f);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}