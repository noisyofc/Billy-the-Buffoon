using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Projectile : MonoBehaviour
{
    [Header("Floats")]
    public float spawnY = 0.05f;
    public float spawnYshelf = 0.7f;

    public GameObject glueWall;
    private GameObject glued;

    private new Vector3 position;
    private Quaternion rotation;

    private Rigidbody rb;

    public GameObject playerObject;

    public Transform[] supportPoints;  // Define support points (corners or key points)
    private LayerMask platformLayer;    // Define what the platform layer is
    public float fallThreshold = 0.5f; // Define threshold for how much of the trampoline needs to be unsupported - 50% of POINTS have to raycast!!!

    //private Rigidbody trampolineRb;    // Reference to the trampoline's Rigidbody
    private bool isFalling = false;

    private Renderer[] Mesh;

    private void Start()
    {
        platformLayer = LayerMask.GetMask("whatIsGround");

        // get rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, 0, 0);


        Mesh = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in Mesh)
        {
            renderer.enabled = false;
        }


        playerObject = GameObject.FindGameObjectWithTag("Player");

    }

    private void Update()
    {
        Vector3 player_pos = playerObject.transform.position;
        
        float dist = Vector3.Distance(playerObject.transform.position, transform.position);
        //Debug.Log(dist);
        if (gameObject.transform.tag == "trampRdy" && dist > 1)
        {
            foreach (Renderer renderer in Mesh)
            {
                renderer.enabled = true;
            }
        }

        else if (gameObject.transform.tag == "bananaRdy" && dist > 1)
        {
            foreach (Renderer renderer in Mesh)
            {
                renderer.enabled = true;
            }
        }

        else if (gameObject.transform.tag == "glueRdy" && dist > 1)
        {
            foreach (Renderer renderer in Mesh)
            {
                renderer.enabled = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (Renderer renderer in Mesh)
        {
            renderer.enabled = true;
        }
        if (collision.gameObject.transform.tag == "floor" && isFalling == false)
        {
            //Destroy(gameObject);
            bool result = CheckSupport();

            Collider playerCollider = playerObject.GetComponent<Collider>();
            Collider projectileCollider = GetComponent<Collider>();
            Physics.IgnoreCollision(projectileCollider, playerCollider, false);

            gameObject.transform.position = new Vector3(transform.position.x, collision.transform.position.y + spawnY, transform.position.z);
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f));
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = true;
            StartCoroutine(countDown());

            if (gameObject.transform.tag == "trampRdy")
                {
                    gameObject.transform.tag = "tramp";
                }

                if (gameObject.transform.tag == "bananaRdy")
                {
                    gameObject.transform.tag = "banana";
                }

            if (result)
            {
                isFalling = true;

                gameObject.transform.position = new Vector3(transform.position.x, collision.transform.position.y + spawnY + ((collision.transform.localScale.y - 1) / 2), transform.position.z);
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f));
                rb.constraints = RigidbodyConstraints.FreezePosition;

                StartCoroutine(countDownFall());

                EnableFalling();
                rb.constraints = RigidbodyConstraints.None;
            }

        }

        if (collision.gameObject.transform.tag == "Wall" && gameObject.transform.tag == "glueRdy")
        {
            position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rotation = Quaternion.Euler(new Vector3(collision.transform.rotation.eulerAngles.x, collision.transform.rotation.eulerAngles.y));
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = true;

            collision.gameObject.transform.tag = "glued";

            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            collision.gameObject.transform.GetChild(1).gameObject.SetActive(true);

            //GameObject glued = Instantiate(glueWall, position, rotation);
            //glued.gameObject.transform.localScale = collision.gameObject.transform.localScale;
            Destroy(gameObject);
        }

        if (collision.gameObject.transform.tag == "floor" && gameObject.transform.tag == "glueRdy")
        {

            Destroy(gameObject);
        }

    }

    IEnumerator countDown()
    {
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    IEnumerator countDownFall ()
    {
        yield return new WaitForSeconds(0.5f);
    }

    private bool CheckSupport()
    {
        int supportedPoints = 0;

        // Cast raycasts from each support point to check if they are on the platform
        foreach (Transform point in supportPoints)
        {
            // Raycast down to check if the point is supported by a platform
            if (Physics.Raycast(point.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, platformLayer))
            {
                supportedPoints++;
            }
        }

        // Calculate the percentage of support
        float supportPercentage = (float)supportedPoints / supportPoints.Length;

        // If the trampoline is not sufficiently supported, allow it to fall
        if (supportPercentage < fallThreshold)
        {
            Debug.Log("Trampoline is not supported, falling...");
            //EnableFalling();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EnableFalling()
    {
        rb.isKinematic = false;  // Allow physics to take over
        rb.useGravity = true;    // Ensure gravity is applied
    }

}