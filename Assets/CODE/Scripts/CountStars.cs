using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountStars : MonoBehaviour
{

    public int stars = 0;
    public GameObject[] StarsObj;

    public static CountStars instance;  

    // Start is called before the first frame update
    void Start()
    {
        stars = 0;
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {


        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            stars += 1;
            StarsObj[stars-1].SetActive(true);
        }
    }

}
