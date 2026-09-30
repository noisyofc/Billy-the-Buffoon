using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Object Spawn Settings")]
    public float spawnY = 0.05f;
    public float spawnYshelf = 0.7f;

    [Header("Glue Wall Settings")]
    public GameObject glueWall;
    private GameObject glued;

    [Header("Glue Wall Placement Variables")]
    private Vector3 position;
    private Quaternion rotation;

    [Header("Trampoline Support Settings")]
    public Transform[] supportPoints;
    public LayerMask platformLayer;
    public float fallThreshold = 0.5f;
    public float rayLength = 1.0f;

    [Header("Player Reference")]
    [SerializeField] private GameObject playerObject;

    private Rigidbody rb;
    private Renderer[] meshRenderers;
    private bool isFalling;
    private bool _visibilityResolved;
    private Collider _playerCollider;
    private Collider _projectileCollider;

    public GameObject[] glueParticles;

    private void Start()
    {
        platformLayer = LayerMask.GetMask("whatIsGround", "whatIsPlayer");

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        meshRenderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].enabled = false;

        if (playerObject == null)
            playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            _playerCollider = playerObject.GetComponent<Collider>();
        _projectileCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_visibilityResolved || playerObject == null)
        {
            enabled = false;
            return;
        }

        if (!CompareTag("trampRdy") && !CompareTag("bananaRdy") && !CompareTag("glueRdy"))
            return;

        float distSq = (playerObject.transform.position - transform.position).sqrMagnitude;
        if (distSq <= 1f)
            return;

        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].enabled = true;

        _visibilityResolved = true;
        enabled = false;
    }

    private void SpawnHitParticles(Vector3 point)
    {
        SimpleParticlePool.PlayAll(glueParticles, point, rotation, this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].enabled = true;
        _visibilityResolved = true;
        enabled = false;

        bool hitFloor = collision.gameObject.CompareTag("floor");
        bool hitMovingPlatform = collision.gameObject.CompareTag("ruchomaPlatforma");
        bool hitWall = collision.gameObject.CompareTag("Wall");

        if ((hitFloor || hitMovingPlatform) && !isFalling)
        {
            if (TryGetBestUpwardContact(collision, out ContactPoint contact))
            {
                SpawnHitParticles(contact.point);

                // Snap first — support rays are unreliable while the throw is still tilted.
                SnapUprightOnPlatform(collision, contact, hitMovingPlatform);

                if (CompareTag("trampRdy"))
                    tag = "tramp";
                else if (CompareTag("bananaRdy"))
                    tag = "banana";

                bool shouldFall = CheckSupport();
                if (shouldFall)
                {
                    isFalling = true;
                    StartCoroutine(CountDownFall());
                    EnableFalling();
                }
                else if (hitMovingPlatform)
                {
                    MovingPlatform platform = collision.gameObject.GetComponentInParent<MovingPlatform>();
                    if (platform != null)
                        platform.AttachProp(transform);
                }

                if (CompareTag("tramp"))
                {
                    // Under-foot throw: IgnoreCollision is still on, so OnCollisionEnter never fires — bounce manually.
                    TryBouncePlayerOnPlacedTramp();
                }
                else if (_projectileCollider != null && _playerCollider != null)
                {
                    // Banana etc.: wait until clear so the player doesn't shove the prop.
                    StartCoroutine(EnablePlayerCollisionWhenClear());
                }

                StartCoroutine(CountDown());
            }
        }

        if (hitWall && CompareTag("glueRdy"))
        {
            position = transform.position;
            rotation = Quaternion.Euler(collision.transform.rotation.eulerAngles);

            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = true;

            collision.gameObject.tag = "glued";
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            collision.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            GlueRegistry.MarkDirty();

            SpawnHitParticles(collision.contacts[0].point);
            StartCoroutine(CountDown());
        }

        if (hitFloor && CompareTag("glueRdy"))
        {
            SpawnHitParticles(collision.contacts[0].point);
            StartCoroutine(CountDown());
        }

        // Misc surfaces only — do not double-fire for floor / wall / moving platforms.
        if (!hitFloor && !hitWall && !hitMovingPlatform)
        {
            SpawnHitParticles(collision.contacts[0].point);
            StartCoroutine(CountDown());
        }

        if (collision.gameObject.CompareTag("ocean"))
            StartCoroutine(CountDown());
    }

    private static bool TryGetBestUpwardContact(Collision collision, out ContactPoint best)
    {
        best = default;
        bool found = false;
        float bestY = 0.5f;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint c = collision.GetContact(i);
            if (c.normal.y >= bestY)
            {
                bestY = c.normal.y;
                best = c;
                found = true;
            }
        }

        return found;
    }

    /// <summary>
    /// Places tramp/banana upright on the hit surface. Uses a down-ray so elevators
    /// don't get a stale contact point from mid-motion collision.
    /// </summary>
    private void SnapUprightOnPlatform(Collision collision, ContactPoint contact, bool onMovingPlatform)
    {
        float yaw = transform.eulerAngles.y;
        Quaternion upright = Quaternion.Euler(0f, yaw, 0f);

        Vector3 surfacePoint = ResolveSurfacePoint(collision, contact, onMovingPlatform);
        Vector3 pos = surfacePoint + Vector3.up * spawnY;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.position = pos;
            rb.rotation = upright;
            // Moving-platform props lose their Rigidbody in AttachProp; floor keeps a freeze.
            rb.constraints = onMovingPlatform
                ? RigidbodyConstraints.FreezeRotation
                : RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        transform.SetPositionAndRotation(pos, upright);
    }

    private Vector3 ResolveSurfacePoint(Collision collision, ContactPoint contact, bool onMovingPlatform)
    {
        // Prefer XZ from the contact — projectile transform can still be off to the side mid-bounce.
        Vector3 hint = contact.point;
        int mask = LayerMask.GetMask("whatIsGround");
        MovingPlatform hitPlatform = onMovingPlatform
            ? collision.gameObject.GetComponentInParent<MovingPlatform>()
            : null;

        Vector3 origin = new Vector3(hint.x, hint.y + 2f, hint.z);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 5f, mask, QueryTriggerInteraction.Ignore))
        {
            if (!onMovingPlatform)
                return hit.point;

            MovingPlatform rayPlatform = hit.collider.GetComponentInParent<MovingPlatform>();
            if (rayPlatform != null && (hitPlatform == null || rayPlatform == hitPlatform))
                return hit.point;
        }

        // Fallback: ray from slightly above the projectile XZ.
        origin = new Vector3(transform.position.x, hint.y + 2f, transform.position.z);
        if (Physics.Raycast(origin, Vector3.down, out hit, 5f, mask, QueryTriggerInteraction.Ignore))
        {
            if (!onMovingPlatform)
                return hit.point;

            MovingPlatform rayPlatform = hit.collider.GetComponentInParent<MovingPlatform>();
            if (rayPlatform != null && (hitPlatform == null || rayPlatform == hitPlatform))
                return hit.point;
        }

        return hint;
    }

    private void TryBouncePlayerOnPlacedTramp()
    {
        if (_projectileCollider != null && _playerCollider != null)
            Physics.IgnoreCollision(_projectileCollider, _playerCollider, false);

        if (playerObject == null)
            return;

        PlayerMovementAdvanced movement = playerObject.GetComponent<PlayerMovementAdvanced>();
        if (movement == null)
            return;

        // Always bounce when tramp appears under the player (overlap or very close above).
        float verticalGap = playerObject.transform.position.y - transform.position.y;
        bool nearFeet = verticalGap > -0.5f && verticalGap < playerObject.transform.localScale.y * 2.5f;
        float flatDistSq = new Vector3(
            playerObject.transform.position.x - transform.position.x,
            0f,
            playerObject.transform.position.z - transform.position.z).sqrMagnitude;

        bool overlapping = _projectileCollider != null && _playerCollider != null &&
            Physics.ComputePenetration(
                _projectileCollider, _projectileCollider.transform.position, _projectileCollider.transform.rotation,
                _playerCollider, _playerCollider.transform.position, _playerCollider.transform.rotation,
                out _, out _);

        if (overlapping || (nearFeet && flatDistSq < 1.2f * 1.2f))
            movement.BounceFromTrampoline();
    }

    private IEnumerator EnablePlayerCollisionWhenClear()
    {
        // Wait until the player is no longer overlapping (under-foot throws).
        while (_projectileCollider != null && _playerCollider != null)
        {
            if (!Physics.ComputePenetration(
                    _projectileCollider, _projectileCollider.transform.position, _projectileCollider.transform.rotation,
                    _playerCollider, _playerCollider.transform.position, _playerCollider.transform.rotation,
                    out _, out _))
            {
                Physics.IgnoreCollision(_projectileCollider, _playerCollider, false);
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator CountDown()
    {
        if (CompareTag("glueRdy") || CompareTag("glue"))
        {
            BrakeGlueBottle.PlaySound();
            yield return new WaitForSeconds(0.1f);
            GlueRegistry.MarkDirty();
            Destroy(gameObject);
        }
        else if (CompareTag("trampRdy") || CompareTag("tramp"))
        {
            TrampLanding.PlaySound();
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
        else if (CompareTag("bananaRdy") || CompareTag("banana"))
        {
            BananLand.PlaySound();
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }

    IEnumerator CountDownFall()
    {
        yield return new WaitForSeconds(0.5f);
    }

    private bool CheckSupport()
    {
        if (supportPoints == null || supportPoints.Length == 0)
            return true;

        int supportedPoints = 0;
        for (int i = 0; i < supportPoints.Length; i++)
        {
            Transform point = supportPoints[i];
            if (point == null)
                continue;

            Debug.DrawRay(point.position, Vector3.down * rayLength, Color.red, 2f);
            if (Physics.Raycast(point.position, Vector3.down, out _, rayLength, platformLayer))
                supportedPoints++;
        }

        float supportPercentage = (float)supportedPoints / supportPoints.Length;
        return supportPercentage < fallThreshold;
    }

    private void EnableFalling()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
