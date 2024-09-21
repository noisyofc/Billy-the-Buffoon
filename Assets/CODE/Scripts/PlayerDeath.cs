using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(-8.5f, 16f, -5.5f);
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.transform.tag == "ocean")
        {
            gameObject.transform.position = new Vector3(-8.5f, 16f, -5.5f);
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
    }
}
