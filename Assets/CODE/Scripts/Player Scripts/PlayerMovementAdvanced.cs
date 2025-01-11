using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float crouchSpeed;
    public float stuckSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    [Header("Jump Settings")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    [Header("Physics Settings")]
    public float groundDrag;
    public float dragAir;
    public float playerMass = 1f;
    public float playerMassAir = 0.5f;

    [Header("Player Properties")]
    public float crouchYScale;
    private float startYScale;
    public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Key Bindings")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("UI Elements")]
    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    [Header("References")]
    public Transform orientation;
    public GameObject Umbrella;
    public GameObject[] underSlides;
    public Collider playerCollider;
    public Collider[] underSlidesCollider;
    public bool under = false;  // Tracks if the player is under a specific object
    private RaycastHit slopeHit;
    public float maxSlopeAngle;
    private bool exitingSlope;

    // State and Movement Variables
    public bool grounded, air, stuck, crouching, wallrunning, sliding, freeze;
    private bool enableMovementOnNextTouch;
    private Vector3 moveDirection;
    private bool activeGraple;
    private Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;

    public GameObject optionsCanvas;
    public GameObject mainUI;
    public static bool Paused;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    public Camera mainCamera;

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

    public ParticleSystem slideParticles;

    private void Start()
    {
        // Initialize Rigidbody and prevent rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Initial player scale and settings
        readyToJump = true;
        startYScale = transform.localScale.y;
        playerCollider = GetComponent<Collider>();

        // Find and store objects tagged as underSlides
        underSlides = GameObject.FindGameObjectsWithTag("underStuck");

        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);
    }

    private void Update()
    {
        if (wallrunning == true)
        {
            Umbrella.gameObject.SetActive(false);
        }

        // Pause
#if UNITY_WEBGL
        if (Input.GetButtonDown("Cancel") && !Paused && !EndScreen.endLevel)
#endif
#if !UNITY_WEBGL
        if (Input.GetButtonDown("Pause") && !Paused && !EndScreen.endLevel)
#endif
        {
            Paused = true;
            Time.timeScale = 0;
            optionsCanvas.SetActive(true);
            mainUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            depthOfField.active = true;
        }

        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Handle physics adjustments depending on whether the player is grounded or airborne
        if (grounded && !activeGraple)
        {
            Umbrella.gameObject.SetActive(false);
            rb.drag = groundDrag;
            rb.mass = playerMass;
        }
        else
        {
            Umbrella.gameObject.SetActive(false);
            rb.drag = 0;
            if ((Input.GetButton("Parasol") || Input.GetAxis("Parasol") != 0) && wallrunning == false)
            {
                rb.drag = dragAir;
                rb.mass = playerMassAir;
                StartCoroutine(umbrellaWait()); //wait as umbrella shows for a second
            }
        }
    }

    private void FixedUpdate()
    {

        if (Paused == false)
        {
        MovePlayer();
        }
    }

    private void MyInput()
    {
        // Get player input for movement
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Reset mass and drag when touching the floor or trampoline
        if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("tramp"))
        {
            rb.drag = groundDrag;
            rb.mass = playerMass;
        }

        // Jump when hitting a trampoline
        if (collision.transform.tag == "tramp")
        {
            Jump();
        }

        // Handle banana collision for crouching and sliding behavior
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
        // Stop sliding when exiting a banana collision
        if (collision.transform.tag == "banana")
        {
            StartCoroutine(stopSlide());
        }
    }

    private void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else if (wallrunning)
        {
            Umbrella.gameObject.SetActive(false);
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
            rb.useGravity = false;  // Disable gravity during wall running
        }
        else if (stuck)
        {
            HandleStuckState();
            
        }
        else if (crouching)
        {
            HandleCrouchState();
        }
        else if (grounded)
        {
            state = MovementState.walking;  // Transition to walking if grounded
            desiredMoveSpeed = walkSpeed;
            rb.useGravity = true;  // Re-enable gravity if grounded
        }
        else
        {
            state = MovementState.air;  // Transition to air state if not grounded
            rb.useGravity = true;  // Re-enable gravity in the air
        }

        // Smoothly adjust speed if there's a big speed change
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 5f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void HandleStuckState()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        float rayDistance = 5.0f;

        // Cast a ray upwards to check if something is blocking above
        if (Physics.Raycast(rayOrigin, Vector3.up, out hit, rayDistance))
        {
            if (hit.collider.gameObject.CompareTag("underStuck"))
            {
                under = true;
                stuck = true;

                // Set the player's scale for crouching
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);

                // Stop movement if stuck under something
                state = MovementState.stuck;
                rb.velocity = Vector3.zero;
            }
            else
            {
                ResetStuckState();
            }
        }
        else
        {
            ResetStuckState();
        }
    }

    private void HandleCrouchState()
    {
        state = MovementState.crouching;
        desiredMoveSpeed = crouchSpeed;

        if (under && stuck)
        {
            // If the player is stuck under an object, stop movement
            state = MovementState.stuck;
            rb.velocity = Vector3.zero;
        }
        else
        {
            // Normal crouch behavior
            desiredMoveSpeed = crouchSpeed;
        }
    }

    private void ResetStuckState()
    {
        // Reset the player's state from being stuck
        under = false;
        stuck = false;
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        crouching = false;
    }

    private void MovePlayer()
    {
        if (activeGraple) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Handle wall running movement
        if (wallrunning)
        {
            // Apply forward force along the wall to keep the player running
            rb.AddForce(moveDirection.normalized * moveSpeed * 20f, ForceMode.Force);

            // Keep the player at a fixed height while wall running by counteracting gravity
            rb.useGravity = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        else
        {
            // Handle movement on a slope
            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else if (!grounded)
            {
                rb.useGravity = true;  // Ensure gravity is enabled while in the air
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }
    }

    private void SpeedControl()
    {
        if (state == MovementState.stuck)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > stuckSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * stuckSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        else if (state == MovementState.stuck)
        {
            rb.velocity = Vector3.zero;  // Stop movement when stuck
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void Jump()
    {
        rb.drag = groundDrag;
        rb.mass = playerMass;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    public void ResetRestricions()
    {
        activeGraple = false;
    }

    IEnumerator stopSlide()
    {
        slideParticles.Play();
        yield return new WaitForSeconds(0.5f);
        HandleStuckState();
        slideParticles.Stop();
    }

    IEnumerator umbrellaWait()
    {
        yield return 0;
        Umbrella.gameObject.SetActive(true);
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
}
