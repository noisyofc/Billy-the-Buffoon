using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Kierunek ruchu (np. (1,0,0) → prawo/lewo, (0,1,0) → góra/dół, (1,1,0) → po skosie)")]
    public Vector3 moveDirection = Vector3.right;

    [Header("Jak daleko platforma się przesunie")]
    public float distance = 3f;

    [Header("Prędkość ruchu")]
    public float speed = 2f;

    private Vector3 startPosition;

    private readonly List<Transform> objectsOnPlatform = new List<Transform>();

    private void Start()
    {
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        float movement = Mathf.PingPong(Time.time * speed, distance * 2) - distance;
        transform.position = startPosition + moveDirection.normalized * movement;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == transform || other.transform.IsChildOf(transform)) return;

        if (!objectsOnPlatform.Contains(other.transform))
        {
            objectsOnPlatform.Add(other.transform);
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectsOnPlatform.Contains(other.transform))
        {
            other.transform.SetParent(null);
            objectsOnPlatform.Remove(other.transform);
        }
    }
}
