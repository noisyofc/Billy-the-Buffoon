using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovementAdvanced : MonoBehaviour
{
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float stuckSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded, air;

    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public float playerMass = 1f;
    public float dragAir = 10f;
    public float playerMassAir = 0.5f;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public GameObject[] underSlides;
    public Collider playerCollider;
    public Collider[] underSLidesCollider;

    public GameObject Umbrella;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air,
        stuck
    }

    public bool freeze;
    public bool sliding;
    public bool crouching;
    public bool wallrunning;

    private bool enableMovementOnNextTouch;

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    public bool under = false;
    private bool stuck = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        playerCollider = GetComponent<Collider>();
        underSlides = GameObject.FindGameObjectsWithTag("glued");
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded && !activeGraple)
        {
            Umbrella.gameObject.SetActive(false);
            rb.drag = groundDrag;           // Reset drag when grounded
            rb.mass = playerMass;           // Reset mass when grounded
        }
        else
        {
            Umbrella.gameObject.SetActive(false);
            rb.drag = 0;
            if (Input.GetKey(jumpKey))
            {
                Umbrella.gameObject.SetActive(true);
                rb.drag = dragAir;           // Set drag for umbrella use
                rb.mass = playerMassAir;     // Set mass for umbrella use
            }
        }
    }

    public void ResetRestricions()
    {
        activeGraple = false;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestricions();

            GetComponent<Grappling>().StopGrapple();
        }

        // Reset mass and drag when touching the ground or trampoline
        if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("tramp"))
        {
            rb.drag = groundDrag;       // Reset drag to ground drag
            rb.mass = playerMass;        // Reset mass to normal player mass
        }

        if (collision.transform.tag == "tramp")
        {
            Jump();
        }

        if (collision.transform.tag == "banana")
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            rb.AddForce(Vector3.forward * 5f, ForceMode.Impulse);

            crouching = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "banana")
        {
            StartCoroutine(stopSlide());
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }
        else if (stuck)
        {
            // Raycast to check if there's something "glued" above the player
            RaycastHit hit;
            Vector3 rayOrigin = transform.position;  // Origin of the ray (e.g., from the player's position)
            float rayDistance = 5.0f;                // The distance of the raycast

            // Cast a ray upwards from the player's position
            if (Physics.Raycast(rayOrigin, Vector3.up, out hit, rayDistance))
            {
                if (hit.collider.gameObject.CompareTag("glued"))
                {
                    Debug.Log("Hit an object with tag 'glued'");
                    under = true;
                    stuck = true;
                }
                else
                {
                    Debug.Log("Hit an object without the 'glued' tag");
                    under = false;
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                    crouching = false;
                    stuck = false;

                }
            }
            else
            {
                Debug.Log("No object hit");
                under = false;
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                crouching = false;
                stuck = false;

            }
        }
        // Mode - Crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;

            // When crouching, check for "glued" objects to determine if stuck
            if (under == true && stuck == true)
            {
                // Set the player's scale for crouching
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);

                // Set the movement state to stuck
                state = MovementState.stuck;

                // If there's no input, stop the player from sliding
                if (horizontalInput == 0 && verticalInput == 0)
                {
                    // Set velocity to zero if no input
                    rb.velocity = Vector3.zero;
                }
                else
                {
                    // Apply the stuck speed if there is input
                    moveSpeed = stuckSpeed;
                    rb.velocity = rb.velocity.normalized * stuckSpeed;
                }
            }
            else
            {
                // Normal crouch behavior without being stuck
                desiredMoveSpeed = crouchSpeed;
            }
        }
        

        // Mode - Walking (normal behavior when grounded and not stuck)
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        // Check if the move speed has drastically changed and smooth the transition if needed
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 5f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());

            print("Lerp Started!");
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }



    IEnumerator stopSlide()
    {
        yield return new WaitForSeconds(0.5f); // Wait for the slide to finish

        // Raycast to check if there's something "glued" above the player
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;  // Origin of the ray (e.g., from the player's position)
        float rayDistance = 5.0f;                // The distance of the raycast

        // Cast a ray upwards from the player's position
        if (Physics.Raycast(rayOrigin, Vector3.up, out hit, rayDistance))
        {
            if (hit.collider.gameObject.CompareTag("glued"))
            {
                Debug.Log("Hit an object with tag 'glued'");
                under = true;
                stuck = true;
            }
            else
            {
                Debug.Log("Hit an object without the 'glued' tag");
                under = false;
                stuck = false;
            }
        }
        else
        {
            Debug.Log("No object hit");
            under = false;
            stuck = false;
        }

        // Update the player's state after the raycast check
        if (under == true && stuck == true)
        {
            // Set the player's scale for crouching
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);

            // Set the movement state to stuck
            state = MovementState.stuck;

            // Stop the player from sliding
            rb.velocity = Vector3.zero;
        }
        else
        {
            // Reset player's scale and allow normal movement again
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            crouching = false;
            stuck = false;  // Set stuck to false so they can move normally again
        }
    }


    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {

        if (activeGraple) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        if (!wallrunning) rb.useGravity = !OnSlope();
    }

private void SpeedControl()
{
    if (activeGraple) return;

    // limiting speed on slope
    if (OnSlope() && !exitingSlope)
    {
        if (rb.velocity.magnitude > moveSpeed)
            rb.velocity = rb.velocity.normalized * moveSpeed;
    }
    else if (state == MovementState.stuck)
    {
        // Clamp velocity to stuckSpeed when in the stuck state
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > stuckSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * stuckSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    // limiting speed on ground or in air
    else
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}

    private void Jump()
    {
        // Reset Rigidbody properties before applying jump force
        rb.drag = groundDrag;
        rb.mass = playerMass;

        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Apply the jump force
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }


    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    bool activeGraple;

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGraple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestricions), 3f);
    }

    private void IgnoreCollisionsWithGluedObjects()
    {
        // Loop through each glued object
        foreach (GameObject underSlide in underSlides)
        {
            // Get the Collider component of the current glued object
            Collider underSLidesCollider = underSlide.GetComponent<Collider>();

            // Check if the glued object has a Collider
            if (underSLidesCollider != null)
            {
                // Ignore the collision between the player and the current glued object
                Physics.IgnoreCollision(playerCollider, underSLidesCollider, true);
            }
            else
            {
                Debug.LogWarning("Object with tag 'glued' does not have a Collider component: ");
            }
        }
    }

}
