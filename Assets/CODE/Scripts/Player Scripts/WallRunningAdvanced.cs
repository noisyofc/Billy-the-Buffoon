using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunningAdvanced : MonoBehaviour
{
    [Header("Layer Masks")]
    [Tooltip("Layer that defines what counts as a wall.")]
    public LayerMask whatIsWall;
    [Tooltip("Layer that defines what counts as the ground.")]
    public LayerMask whatIsGround;

    [Header("Wall Running Settings")]
    [Tooltip("Force applied when running on a wall.")]
    public float wallRunForce = 20f;
    [Tooltip("Upwards force applied when jumping off a wall.")]
    public float wallJumpUpForce = 5f;
    [Tooltip("Side force applied when jumping off a wall.")]
    public float wallJumpSideForce = 10f;
    [Tooltip("Speed for climbing up or down the wall.")]
    public float wallClimbSpeed = 5f;
    [Tooltip("Maximum time the player can wall run.")]
    public float maxWallRunTime = 5f;
    private float wallRunTimer;

    [Header("Wall Detection Settings")]
    [Tooltip("The distance from the player to check for walls.")]
    public float wallCheckDistance = 1.5f;
    [Tooltip("Minimum height required to jump.")]
    public float minJumpHeight = 1.0f;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exit Wall Run Settings")]
    [Tooltip("Time to remain in 'exiting wall' state after stopping wall running.")]
    public float exitWallTime = 0.2f;
    private bool exitingWall = false;
    private float exitWallTimer;

    [Header("Gravity Settings")]
    [Tooltip("Whether to use gravity during the wall run.")]
    public bool useGravity = true;
    [Tooltip("Counteracting gravity force while wall running.")]
    public float gravityCounterForce = 10f;

    [Header("Input Settings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;

    [Header("References")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovementAdvanced pm;  // Reference to PlayerMovementAdvanced script
    private Rigidbody rb;

    private GameObject[] walls;

    private float horizontalInput;
    private float verticalInput;
    private bool upwardsRunning;
    private bool downwardsRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();             // Get the Rigidbody component
        pm = GetComponent<PlayerMovementAdvanced>(); // Get reference to PlayerMovementAdvanced script
    }

    private void Update()
    {
        CheckForWall();   // Check if there are walls on either side
        StateMachine();   // Handle wall run state transitions
    }

    private void FixedUpdate()
    {
        // Handle wall running movement in FixedUpdate for physics consistency
        if (pm.wallrunning)
            WallRunningMovement();
    }

    /// <summary>
    /// Checks for walls on the player's left and right using raycasts.
    /// </summary>
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    /// <summary>
    /// Checks if the player is above the minimum height required for a wall jump.
    /// </summary>
    /// <returns>True if the player is above the minimum jump height, otherwise false.</returns>
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    /// <summary>
    /// Handles the wall running state machine logic, transitions between wall running, exiting, and idle states.
    /// </summary>
    private void StateMachine()
    {
        // Get player inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1: Wall Running
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning) StartWallRun();

            // Handle wall run timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;
            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            // Wall jump
            if (Input.GetKeyDown(jumpKey) || Input.GetAxis("Tramp") != 0) WallJump();
        }

        // State 2: Exiting Wall Run
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            // Count down exit wall timer
            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        // State 3: Idle (not wall running)
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    /// <summary>
    /// Initiates wall running, applies camera effects, and sets the wall run timer.
    /// </summary>
    private void StartWallRun()
    {
        // Check if any "glue" walls are active before starting wall run
        walls = GameObject.FindGameObjectsWithTag("glue");

        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i].gameObject.activeInHierarchy)
            {
                pm.wallrunning = true;
                wallRunTimer = maxWallRunTime;  // Start the wall run timer

                // Zero out vertical velocity for smoother transition
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                // Apply camera effects for wall running
                cam.DoFov(90f);
                if (wallLeft) cam.DoTilt(-5f);
                if (wallRight) cam.DoTilt(5f);
            }
        }
    }

    /// <summary>
    /// Handles player movement while wall running, including gravity and directional forces.
    /// </summary>
    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        // Determine which wall we're running on (left or right)
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        // Determine the forward direction while on the wall
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // Apply forward force along the wall
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Apply upward or downward force when climbing or descending
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        // Push the player towards the wall to keep them attached
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        // Counter gravity while wall running
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    /// <summary>
    /// Stops the wall running state and resets the camera effects.
    /// </summary>
    private void StopWallRun()
    {
        pm.wallrunning = false;

        // Reset camera effects
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    /// <summary>
    /// Performs a wall jump, applying upwards and sideways force.
    /// </summary>
    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        // Calculate the force for the wall jump
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Reset vertical velocity and apply the wall jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
