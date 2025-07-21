using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orientation : MonoBehaviour
{
    private Transform orientator;
    private GameObject player;

    void Start()
    {
        orientator = GameObject.Find("Orientation").GetComponent<Transform>();
        player = GameObject.Find("Player");

        orientator.position = player.transform.position;
    }
}
