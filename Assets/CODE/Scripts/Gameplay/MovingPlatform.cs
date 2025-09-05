using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("DODAĆ RIGIDBODY DO PLATFORMY I USTWAIĆ isKinematic!" )]

    [Header("Ustawienia ruchu (np. (1,0,0) → prawo/lewo, (0,1,0) → góra/dół, (1,1,0) → po skosie)")]
    public Vector3 moveDirection = new Vector3(1, 0, 0);

    [Header("jak daleko platforma się przesunie")]
    public float distance = 3f;

    [Header("prędkość ruchu")]
    public float speed = 2f;

    private Vector3 startPosition;
    private Vector3 lastPosition;

    private void Start()
    {
        startPosition = transform.position;
        lastPosition = startPosition;
    }

    private void FixedUpdate()
    {
        float movement = Mathf.PingPong(Time.time * speed, distance * 2) - distance;
        transform.position = startPosition + moveDirection.normalized * movement;
    }

    private void LateUpdate()
    {
        lastPosition = transform.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null && !rb.isKinematic)
        {
            Vector3 platformMovement = transform.position - lastPosition;

            rb.MovePosition(rb.position + platformMovement);
        }
    }
}
