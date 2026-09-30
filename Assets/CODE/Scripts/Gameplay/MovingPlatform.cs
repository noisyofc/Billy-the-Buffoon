using UnityEngine;

/// <summary>
/// Moves along an axis (ping-pong). Player rides by matching <see cref="CurrentVelocity"/>.
/// Placed props are parented (no own Rigidbody) so they follow interpolated platform motion
/// without tilt / sink artifacts on vertical elevators.
/// </summary>
[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Kierunek ruchu (np. (1,0,0) → prawo/lewo, (0,1,0) → góra/dół)")]
    public Vector3 moveDirection = Vector3.right;

    [Header("Jak daleko platforma się przesunie")]
    public float distance = 3f;

    [Header("Prędkość ruchu")]
    public float speed = 2f;

    /// <summary>World-space velocity this FixedUpdate.</summary>
    public Vector3 CurrentVelocity { get; private set; }

    private Rigidbody _rb;
    private Vector3 _startPosition;

    private void Awake()
    {
        // Nested child Rigidbodies block collider compounding and break smooth interpolation.
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i].gameObject == gameObject)
                continue;
            Destroy(bodies[i]);
        }

        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody>();

        _rb.isKinematic = true;
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    private void Start()
    {
        _startPosition = _rb.position;
    }

    private void FixedUpdate()
    {
        float movement = Mathf.PingPong(Time.fixedTime * speed, distance * 2f) - distance;
        Vector3 newPos = _startPosition + moveDirection.normalized * movement;
        Vector3 delta = newPos - _rb.position;

        float dt = Time.fixedDeltaTime;
        CurrentVelocity = dt > 0f ? delta / dt : Vector3.zero;

        _rb.MovePosition(newPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.transform.IsChildOf(transform))
            return;

        Rigidbody body = other.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (!body.CompareTag("Player") && !other.CompareTag("Player"))
            return;

        PlayerMovementAdvanced player = body.GetComponent<PlayerMovementAdvanced>();
        if (player != null)
            player.SetRidingPlatform(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null)
            return;

        Rigidbody body = other.attachedRigidbody;
        if (body == null)
            return;

        PlayerMovementAdvanced player = body.GetComponent<PlayerMovementAdvanced>();
        if (player != null)
            player.ClearRidingPlatform(this);
    }

    /// <summary>
    /// Parents a placed tramp/banana so it rides with the platform transform.
    /// Removes the prop Rigidbody to avoid nested-RB tilt/sink on elevators.
    /// </summary>
    public void AttachProp(Transform prop)
    {
        if (prop == null || prop.IsChildOf(transform))
            return;

        float yaw = prop.eulerAngles.y;
        Quaternion upright = Quaternion.Euler(0f, yaw, 0f);
        Vector3 worldPos = prop.position;
        Vector3 worldScale = prop.lossyScale;

        Rigidbody propRb = prop.GetComponent<Rigidbody>();
        if (propRb != null)
        {
            propRb.velocity = Vector3.zero;
            propRb.angularVelocity = Vector3.zero;
            propRb.isKinematic = true;
            propRb.interpolation = RigidbodyInterpolation.None;
            // Collider becomes part of this platform's compound body and follows the parent.
            Destroy(propRb);
        }

        prop.SetParent(transform, true);
        prop.SetPositionAndRotation(worldPos, upright);

        Vector3 parentScale = transform.lossyScale;
        if (Mathf.Abs(parentScale.x) > 0.0001f &&
            Mathf.Abs(parentScale.y) > 0.0001f &&
            Mathf.Abs(parentScale.z) > 0.0001f)
        {
            prop.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z);
        }

        // Re-assert upright after scale/parent (avoids inherited pitch from mesh children).
        prop.rotation = upright;
        prop.position = worldPos;
    }
}
