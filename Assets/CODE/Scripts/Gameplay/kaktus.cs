using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kaktus : MonoBehaviour
{
    public float pushForce = 55f;
    private Rigidbody player;

    public void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Rigidbody>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player != null)
            {                
                Vector3 pushDirection = collision.transform.position - transform.position;
                pushDirection.y = 0;
                pushDirection.Normalize();

                player.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }
    }
}
