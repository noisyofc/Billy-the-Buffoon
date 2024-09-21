using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeTagOfWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.tag = "Wall";
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.tag == "glued")
        {
            StartCoroutine(countDown());
        }
    }


    IEnumerator countDown()
    {
        yield return new WaitForSeconds(2f);

        gameObject.transform.tag = "Wall";
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

}
