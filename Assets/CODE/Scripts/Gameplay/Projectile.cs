using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Object Spawn Settings")]
    [Tooltip("Y offset when the object is spawned on a platform")]
    public float spawnY = 0.05f;
    [Tooltip("Y offset when the object is spawned on a shelf")]
    public float spawnYshelf = 0.7f;

    [Header("Glue Wall Settings")]
    [Tooltip("Prefab for glue wall")]
    public GameObject glueWall;
    private GameObject glued;

    [Header("Glue Wall Placement Variables")]
    private Vector3 position;         // Stores position for glue wall placement
    private Quaternion rotation;      // Stores rotation for glue wall placement

    [Header("Trampoline Support Settings")]
    [Tooltip("Support points of the trampoline to check for platform support")]
    public Transform[] supportPoints;  // Define support points (corners or key points)
    [Tooltip("Layer mask used to identify platform layer")]
    public LayerMask platformLayer;   // Define what the platform layer is  // Define what the platform layer is
    [Tooltip("Threshold for how much of the trampoline needs to be supported - 50% of points should hit platform")]
    public float fallThreshold = 0.5f; // Define threshold for how much of the trampoline needs to be unsupported
    [Tooltip("Maximum distance for the raycast to check if the support points are on a platform")]
    public float rayLength = 1.0f;     // Maximum distance for the raycast

    [Header("Player Reference")]
    [Tooltip("Reference to the player object")]
    public GameObject playerObject;

    private Rigidbody rb;  // Rigidbody of the projectile
    private Renderer[] meshRenderers;  // Mesh renderers to show/hide object
    private bool isFalling = false;  // Indicates whether the trampoline is falling

    public GameObject[] glueParticles;

    private void Start()
    {
        // Get the platform layer mask (whatIsGround layer)
        platformLayer = LayerMask.GetMask("whatIsGround", "whatIsPlayer");

        // Get the Rigidbody component and center of mass
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;

        // Get all the Mesh Renderers for visibility control
        meshRenderers = GetComponentsInChildren<Renderer>();

        // Initially disable all renderers
        foreach (Renderer renderer in meshRenderers)
        {
            renderer.enabled = false;
        }

        // Find the player object by tag
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // Calculate the distance between the player and the object
        float dist = Vector3.Distance(playerObject.transform.position, transform.position);

        // Show renderers when the object is ready and the player is far enough away
        if ((gameObject.transform.tag == "trampRdy" && dist > 1) ||
            (gameObject.transform.tag == "bananaRdy" && dist > 1) ||
            (gameObject.transform.tag == "glueRdy" && dist > 1))
        {
            foreach (Renderer renderer in meshRenderers)
            {
                renderer.enabled = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Enable all renderers upon collision
        foreach (Renderer renderer in meshRenderers)
        {
            renderer.enabled = true;
        }

        // Handle collision with the floor
        if (collision.gameObject.CompareTag("floor") && !isFalling)
        {
            // Check if the collision was with the top of the floor (normal.y > 0.5)
            ContactPoint contact = collision.contacts[0]; // Get the first contact point
            if (contact.normal.y > 0.5f)
            {
                bool result = CheckSupport();  // Check if the trampoline is sufficiently supported

                // Ignore collision between projectile and player after placement
                Collider playerCollider = playerObject.GetComponent<Collider>();
                Collider projectileCollider = GetComponent<Collider>();
                Physics.IgnoreCollision(projectileCollider, playerCollider, false);

                // Place the projectile on top of the surface (adjusted by spawnY)
                gameObject.transform.position = new Vector3(transform.position.x, contact.point.y + spawnY, transform.position.z);
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0f, transform.rotation.z));

                // Freeze the object in place
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                rb.isKinematic = true;

                // Start countdown before the object is destroyed
                StartCoroutine(CountDown());

                // Change tag for specific object types
                if (gameObject.transform.tag == "trampRdy")
                {
                    gameObject.transform.tag = "tramp";
                }
                else if (gameObject.transform.tag == "bananaRdy")
                {
                    gameObject.transform.tag = "banana";
                }

                // If the object is not sufficiently supported, make it fall
                if (result)
                {
                    isFalling = true;
                    StartCoroutine(CountDownFall());
                    EnableFalling();
                }
            }
        }

        // Handle collision with walls
        if (collision.gameObject.CompareTag("Wall") && gameObject.transform.CompareTag("glueRdy"))
        {
            // Store current position and rotation for glue wall
            position = transform.position;
            rotation = Quaternion.Euler(collision.transform.rotation.eulerAngles);

            // Freeze the object in place
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = true;

            // Set the wall as glued
            collision.gameObject.transform.tag = "glued";
            
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            collision.gameObject.transform.GetChild(1).gameObject.SetActive(true);

            ContactPoint contact = collision.contacts[0]; // Get the first contact point

            for (int i = 0; i < glueParticles.Length; i++)
            {
                Instantiate(glueParticles[i], contact.point, rotation);
            }
            

            // Destroy the projectile once glued
            Destroy(gameObject);
        }

        // If the object is in glueRdy state and hits the floor, destroy it
        if (collision.gameObject.CompareTag("floor") && gameObject.transform.CompareTag("glueRdy"))
        {
            Destroy(gameObject);
        }

        // Start countdown if the collision object is not a wall or floor
        if (!collision.gameObject.CompareTag("floor") && !collision.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(CountDown());
        }

        if (collision.gameObject.CompareTag("ocean"))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Coroutine to destroy the object after a set delay.
    /// </summary>
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Coroutine for falling logic, providing a short delay before falling.
    /// </summary>
    IEnumerator CountDownFall()
    {
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// Check if the trampoline is sufficiently supported based on raycasts from support points.
    /// </summary>
    /// <returns>True if the trampoline is unsupported and should fall, false otherwise.</returns>
    private bool CheckSupport()
    {
        int supportedPoints = 0;

        // Cast raycasts from each support point to check if they are on a platform within ray length
        foreach (Transform point in supportPoints)
        {
            // Debug to see if raycasts are working
            Debug.DrawRay(point.position, Vector3.down * rayLength, Color.red, 2f);  // Visualize raycast in the scene

            if (Physics.Raycast(point.position, Vector3.down, out RaycastHit hit, rayLength, platformLayer))
            {
                supportedPoints++;
            }
        }

        // Calculate support percentage
        float supportPercentage = (float)supportedPoints / supportPoints.Length;

        // Return true if less than the threshold percentage of points are supported
        return supportPercentage < fallThreshold;
    }

    /// <summary>
    /// Enable falling by disabling isKinematic and re-enabling gravity.
    /// </summary>
    private void EnableFalling()
    {
        rb.constraints = RigidbodyConstraints.None;  // Release all constraints
        rb.isKinematic = false;  // Allow physics to take over
        rb.useGravity = true;    // Ensure gravity is applied
    }
}
