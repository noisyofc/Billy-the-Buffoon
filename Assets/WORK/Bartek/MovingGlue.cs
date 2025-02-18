using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGlue : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 targetPos;
    public float speed = 1f; // Speed of the movement
    private float timeElapsed = 0f; // Time elapsed since the start of the movement
    private bool isMoving = false; // Indicates whether the object is moving
    public int whichDrop;
    // Start is called before the first frame update
    void Start()
    {
    // Initialize startPos and targetPos if needed
        startPos = transform.position;
        
        GameObject[] glueParticles = GameObject.FindGameObjectsWithTag("glueParticle");
        foreach (GameObject glueParticle in glueParticles)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), glueParticle.GetComponent<Collider>());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "glueParticle")
        {
            Debug.Log("Collision with glue particle"); 
        }

        Debug.Log("Collision");
        ContactPoint contact = collision.contacts[0];
        startPos = contact.point;
        timeElapsed = 0f; // Reset time elapsed on collision

        // Get the bounds of the collision object
        Bounds bounds = collision.collider.bounds;

        gameObject.GetComponent<Collider>().enabled = false;

        // Set targetPos within the bounds of the collision object
        float offset = 5f; // Small offset to ensure targetPos is within bounds

        gameObject.transform.rotation = collision.transform.rotation;
        targetPos = Vector3.zero;
        switch (whichDrop)
        {
            case 0:
                // Move to the bottom-left corner
                targetPos = new Vector3(
                    startPos.x - ((Vector3.Distance(new Vector3(0,0,bounds.min.z), new Vector3(0,0,bounds.max.z)) * MathF.Sin(collision.gameObject.transform.rotation.z))/(2 * MathF.Sin((180 - collision.gameObject.transform.rotation.z)/2))),
                    bounds.min.y + offset,
                    bounds.min.z + offset
                );
                break;
            case 1:
                // Move to the bottom-right corner
                targetPos = new Vector3(
                    startPos.x + ((Vector3.Distance(new Vector3(0,0,bounds.min.z), new Vector3(0,0,bounds.max.z)) * MathF.Sin(collision.gameObject.transform.rotation.z))/(2 * MathF.Sin((180 - collision.gameObject.transform.rotation.z)/2))),
                    bounds.min.y + offset,
                    bounds.max.z - offset
                );
                break;
            case 2:
                // Move to the top-left corner
                targetPos = new Vector3(
                    startPos.x - ((Vector3.Distance(new Vector3(0,0,bounds.min.z), new Vector3(0,0,bounds.max.z)) * MathF.Sin(collision.gameObject.transform.rotation.z))/(2 * MathF.Sin((180 - collision.gameObject.transform.rotation.z)/2))),
                    bounds.max.y - offset,
                    bounds.min.z + offset
                );
                break;
            case 3:
                // Move to the top-right corner
                targetPos = new Vector3(
                    startPos.x + ((Vector3.Distance(new Vector3(0,0,bounds.min.z), new Vector3(0,0,bounds.max.z)) * MathF.Sin(collision.gameObject.transform.rotation.z))/(2 * MathF.Sin((180 - collision.gameObject.transform.rotation.z)/2))),
                    bounds.max.y - offset,
                    bounds.max.z - offset
                );
                break;
        }

        isMoving = true; // Start moving

        StartCoroutine(CountdownToDisappear());
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            timeElapsed += Time.deltaTime * speed; // Increment time elapsed based on speed

            // Calculate the parabolic path
            float t = timeElapsed;
            float height = 5f; // Maximum height of the parabola
            Vector3 currentPosition = Vector3.Lerp(startPos, targetPos, t);
            currentPosition.x += height * (1 - 4 * (t - 0.5f) * (t - 0.5f)); // Parabolic equation

            // Move the object
            transform.position = currentPosition;

            // Stop the movement when the target position is reached
            if (t >= 1f)
            {
                transform.position = targetPos;
                isMoving = false; // Stop moving
            }
        }
    }

    IEnumerator CountdownToDisappear()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
