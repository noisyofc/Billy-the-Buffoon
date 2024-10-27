using System.Collections;
using UnityEngine;

public class ThrowingTutorial : MonoBehaviour
{
    [Header("Throw Settings")]
    [Tooltip("Array of objects that can be thrown.")]
    public GameObject[] objectsToThrow;  // List of objects that the player can throw
    [Tooltip("Total number of throws the player can make.")]
    public int totalThrows = 3;  // Number of throws available
    [Tooltip("Cooldown time between throws.")]
    public float throwCooldown = 1f;  // Time between throws

    [Header("Throwing Force Settings")]
    [Tooltip("Force applied to the object when thrown.")]
    public float throwForce = 10f;  // Forward force for the throw
    [Tooltip("Upward force applied to the object when thrown.")]
    public float throwUpwardForce = 2f;  // Upward force to create an arc

    [Header("Key Bindings")]
    [Tooltip("Key for throwing trampoline objects.")]
    public KeyCode trampKey = KeyCode.Mouse0;  // Key for throwing trampoline
    [Tooltip("Key for throwing banana objects.")]
    public KeyCode bananaKey = KeyCode.Mouse2;  // Key for throwing banana
    [Tooltip("Key for throwing glue objects.")]
    public KeyCode glueKey = KeyCode.Mouse1;  // Key for throwing glue

    [Header("References")]
    [Tooltip("Player camera transform for direction.")]
    public Transform cam;  // Camera transform used for aiming
    [Tooltip("Position from where the object will be thrown.")]
    public Transform attackPoint;  // Attack point where objects will spawn

    private bool readyToThrow = true;  // Keeps track of whether the player can throw
    private LayerMask platformLayer;   // Layer mask for detecting the ground

    private void Start()
    {
        platformLayer = LayerMask.GetMask("whatIsGround");  // Set the platform layer mask
        Application.targetFrameRate = 60;  // Lock the frame rate to 60 FPS
    }

    private void LateUpdate()
    {
        // Handle throwing based on key input
        if (Input.GetKeyDown(trampKey) && readyToThrow && totalThrows > 0 && PlayerMovementAdvanced.Paused==false)
        {
            Throw(0);  // Throw trampoline object
        }
        if (Input.GetKeyDown(bananaKey) && readyToThrow && totalThrows > 0 && PlayerMovementAdvanced.Paused==false)
        {
            Throw(1);  // Throw banana object
        }
        if (Input.GetKeyDown(glueKey) && readyToThrow && totalThrows > 0 && PlayerMovementAdvanced.Paused==false)
        {
            Throw(2);  // Throw glue object
        }
    }

    /// <summary>
    /// Throws the selected object based on the player's choice.
    /// </summary>
    /// <param name="choice">Index of the object to throw from objectsToThrow array.</param>
    private void Throw(int choice)
    {
        readyToThrow = false;  // Disable throwing until cooldown is over

        // Select the object to throw based on the choice
        GameObject objectToThrow = objectsToThrow[choice];

        // Set the spawn position, and adjust it to be above the ground if necessary
        Vector3 spawnPosition = attackPoint.position;
        RaycastHit groundHit;

        // Raycast to ensure the object spawns above the ground
        if (Physics.Raycast(spawnPosition, Vector3.down, out groundHit, Mathf.Infinity, platformLayer))
        {
            spawnPosition = groundHit.point + Vector3.up * 0.5f;  // Adjust the spawn height
        }

        // Instantiate the object to throw at the attack point with the camera's rotation
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        // Get the Rigidbody component of the projectile
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.interpolation = RigidbodyInterpolation.Interpolate;
        projectileRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Ignore collision with the player
        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider playerCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(projectileCollider, playerCollider);

        // Calculate the direction to throw the object (aim at hit point or default to forward)
        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            // Adjust direction towards hit point
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // Add force to the projectile, combining forward and upward forces
        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        // Reduce total throws and start cooldown before allowing another throw
        totalThrows--;
        Invoke(nameof(ResetThrow), throwCooldown);

        // Debug visualization of the throw direction
        Debug.DrawRay(attackPoint.position, forceDirection * 10, Color.red, 2f);
    }

    /// <summary>
    /// Resets the ability to throw after the cooldown period.
    /// </summary>
    private void ResetThrow()
    {
        readyToThrow = true;  // Allow the player to throw again
    }
}
