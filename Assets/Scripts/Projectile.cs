using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private void Start()
    {
        // get rigidbody component
        rb = GetComponent<Rigidbody>();

        StartCoroutine(countDown());

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "floor")
        {
            //Destroy(gameObject);
            gameObject.transform.position = new Vector3(transform.position.x, collision.transform.position.y + spawnY, transform.position.z);
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f));
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = true;

            if(gameObject.transform.tag == "trampRdy")
            {
                gameObject.transform.tag = "tramp";
            }

            if (gameObject.transform.tag == "bananaRdy")
            {
                gameObject.transform.tag = "banana";
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

}