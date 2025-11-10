using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunningAdvanced : MonoBehaviour
{
    [Header("Layer Masks")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;

    [Header("Wall Running Settings")]
    public float wallRunForce = 20f;
    public float wallJumpUpForce = 5f;
    public float wallJumpSideForce = 10f;
    public float wallClimbSpeed = 5f;
    public float maxWallRunTime = 5f;

    [Header("Wall Detection Settings")]
    public float wallCheckDistance = 1.2f;
    public float wallCheckHeight = 1.0f;     // cast height above player origin
    public float minJumpHeight = 1.0f;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exit Settings")]
    public float exitWallTime = 0.25f;
    private bool exitingWall = false;
    private float exitWallTimer;

    [Header("Gravity & Stick")]
    public bool useGravity = true;
    public float gravityCounterForce = 10f;
    public float stickForce = 100f; // force that keeps player pinned to wall

    [Header("Input")]
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Camera")]
    public PlayerCam cam;
    public float tiltAmount = 6f;
    public float cameraLimitHalfDeg = 25f;

    [Header("References")]
    public Transform orientation; // player's orientation (for forward)
    private PlayerMovementAdvanced pm;
    private Rigidbody rb;

    private float wallRunTimer;
    private float horizontalInput;
    private float verticalInput;
    private bool upwardsRunning;
    private bool downwardsRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();
    }

    private void Update()
    {
        // read inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // detect walls using player's local right/left (not camera)
        CheckForWall();

        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        // more robust side detection using SphereCast at two heights
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 originHigh = transform.position + Vector3.up * wallCheckHeight;
        float sphereRadius = 0.3f;

        RaycastHit hitRightLow, hitRightHigh, hitLeftLow, hitLeftHigh;
        bool rightLow = Physics.SphereCast(origin, sphereRadius, transform.right, out hitRightLow, wallCheckDistance, whatIsWall);
        bool rightHigh = Physics.SphereCast(originHigh, sphereRadius, transform.right, out hitRightHigh, wallCheckDistance, whatIsWall);
        bool leftLow = Physics.SphereCast(origin, sphereRadius, -transform.right, out hitLeftLow, wallCheckDistance, whatIsWall);
        bool leftHigh = Physics.SphereCast(originHigh, sphereRadius, -transform.right, out hitLeftHigh, wallCheckDistance, whatIsWall);

        if (rightHigh) rightWallHit = hitRightHigh;
        else rightWallHit = hitRightLow;

        if (leftHigh) leftWallHit = hitLeftHigh;
        else leftWallHit = hitLeftLow;

        wallRight = rightLow || rightHigh;
        wallLeft = leftLow || leftHigh;
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private bool IsNextToGluedWall(out RaycastHit gluedHit)
    {
        // consider a wall glued if the collider or its root has tag "glue" or "glued"
        gluedHit = new RaycastHit();

        if (wallLeft)
        {
            gluedHit = leftWallHit;
            if (leftWallHit.collider != null && (leftWallHit.collider.CompareTag("glue") || leftWallHit.collider.transform.root.CompareTag("glue")))
                return true;
        }
        if (wallRight)
        {
            gluedHit = rightWallHit;
            if (rightWallHit.collider != null && (rightWallHit.collider.CompareTag("glue") || rightWallHit.collider.transform.root.CompareTag("glue")))
                return true;
        }
        return false;
    }

    private void StateMachine()
    {
        // determine if player is intentionally wallrunning (holding forward + toward wall)
        bool holdingForward = verticalInput > 0.1f;
        bool holdingSide = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) || Mathf.Abs(horizontalInput) > 0.1f;
        bool holdingWallrunKeys = holdingForward && holdingSide;

        // allow stick to glue even without keys
        bool glued = IsNextToGluedWall(out _);

        // If we detect a wall and either the player holds keys or the wall is glued, try to start/continue
        if ((wallLeft || wallRight) && (holdingWallrunKeys || glued) && AboveGround())
        {
            // If exiting due to timer, allow immediate reattach when touching a new wall
            if (exitingWall)
            {
                // small grace: only cancel exiting if we've actually contacted a wall this frame
                exitingWall = false;
            }

            if (!pm.wallrunning)
            {
                StartWallRun();
            }
            else
            {
                // already wallrunning -> refresh timer while actively holding or glued
                if (holdingWallrunKeys || glued)
                    wallRunTimer = maxWallRunTime;

                // if player switched side mid-wallrun, update camera tilt immediately
                if (wallLeft)
                    cam.DoTilt(-tiltAmount);
                else if (wallRight)
                    cam.DoTilt(tiltAmount);
            }

            // if player releases keys and not glued, decrease timer
            if (!holdingWallrunKeys && !glued)
            {
                wallRunTimer -= Time.deltaTime;
                if (wallRunTimer <= 0f)
                {
                    exitingWall = true;
                    exitWallTimer = exitWallTime;
                }
            }

            if (Input.GetKeyDown(jumpKey))
                WallJump();
        }
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0f)
                exitingWall = false;
        }
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;

        // cancel any exit state so we can attach to a new wall immediately
        exitingWall = false;

        // damp vertical velocity for smooth transition
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // camera effects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-tiltAmount);
        else if (wallRight) cam.DoTilt(tiltAmount);

        // enable camera limits but center on current look so player can still move camera within bounds
        cam.EnableCameraLimits(cameraLimitHalfDeg, cameraLimitHalfDeg);
    }

    private void WallRunningMovement()
    {
        // choose which wall hit to use
        RaycastHit hit = wallRight ? rightWallHit : leftWallHit;
        Vector3 wallNormal = hit.normal;

        // forward along the wall
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up).normalized;
        // ensure forward points roughly to player's forward
        if (Vector3.Dot(wallForward, orientation.forward) < 0f)
            wallForward = -wallForward;

        // drive forward/back by vertical input only (lock lateral)
        float forwardInput = Mathf.Clamp(verticalInput, -1f, 1f);

        // desired velocity along wall forward + maintain vertical component control
        Vector3 desiredForwardVel = wallForward * (forwardInput * pm.moveSpeed);
        Vector3 currentUpVel = Vector3.Project(rb.velocity, Vector3.up);

        // apply smoothing/force to reach desired forward velocity
        Vector3 currentForwardVel = Vector3.Project(rb.velocity, wallForward);
        Vector3 forwardDelta = desiredForwardVel - currentForwardVel;
        rb.AddForce(forwardDelta * 10f, ForceMode.Acceleration); // tuning factor

        // allow controlled climb/descend
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        else if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        else
            rb.velocity = new Vector3(rb.velocity.x, currentUpVel.y, rb.velocity.z);

        // push player to wall to keep attached
        rb.AddForce(-wallNormal * stickForce, ForceMode.Force);

        // counter gravity to make wallrun feel stable
        if (useGravity)
            rb.AddForce(Vector3.up * gravityCounterForce, ForceMode.Force);

        // maintain camera tilt while allowing X/Y look changes
        if (wallLeft) cam.DoTilt(-tiltAmount);
        else if (wallRight) cam.DoTilt(tiltAmount);
        else cam.DoTilt(0f);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        cam.DoFov(80f);
        cam.DoTilt(0f);
        cam.DisableCameraLimits();
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        RaycastHit hit = wallRight ? rightWallHit : leftWallHit;
        Vector3 wallNormal = hit.normal;

        Vector3 forceToApply = Vector3.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        // immediately stop wallrun
        StopWallRun();
    }
}

// Note: small helper added - pm.moveSpeed() is expected to return current move speed from PlayerMovementAdvanced.
// If not implemented, either expose moveSpeed as public or replace pm.moveSpeed() with appropriate value.