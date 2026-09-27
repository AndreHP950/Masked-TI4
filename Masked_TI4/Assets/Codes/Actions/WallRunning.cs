using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Detection")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;

    public float wallCheckDistance = 1f;
    public float minJumpHeight = 1f;

    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;

    private bool wallLeft;
    private bool wallRight;

    [Header("Wall Running")]
    public float wallRunForce = 10f;
    public float wallClimbSpeed = 5f;
    public float maxWallRunTime = 3f;

    private float wallRunTimer;

    [Header("Wall Running Keys")]
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;

    private bool upwardsRunning;
    private bool downwardsRunning;

    private float horizontalInput;
    private float verticalInput;

    [Header("Wall Exit")]
    public float exitWallTime = 0.2f;

    private bool exitingWall;
    private float exitWallTimer;

    [Header("Wall Jump")]
    public float wallJumpUpForce = 8f;
    public float wallJumpSideForce = 8f;

    public KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    public Transform orientation;

    private TestePlayerCC pm;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pm = GetComponent<TestePlayerCC>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();

        if (Input.GetKeyDown(jumpKey) && pm.wallrunning)
        {
            WallJump();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(
            transform.position,
            orientation.right,
            out rightWallhit,
            wallCheckDistance,
            whatIsWall
        );

        wallLeft = Physics.Raycast(
            transform.position,
            -orientation.right,
            out leftWallhit,
            wallCheckDistance,
            whatIsWall
        );
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(
            transform.position,
            Vector3.down,
            minJumpHeight,
            whatIsGround
        );
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning)
                StartWallRun();

            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
        }
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }

        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        // Direction along the wall
        Vector3 wallForward = Vector3.Cross(wallNormal,Vector3.up);

        // Make wall direction match player's facing direction
        if (Vector3.Dot( orientation.forward, wallForward) < 0f)
        {
            wallForward = -wallForward;
        }

        // Horizontal wall movement
        Vector3 movement = wallForward * wallRunForce;

        // Climbing / descending
        float verticalMovement = 0f;

        if (upwardsRunning)
        {
            verticalMovement = wallClimbSpeed;
        }
        else if (downwardsRunning)
        {
            verticalMovement = -wallClimbSpeed;
        }

        movement.y = verticalMovement;

        // Keep player attached to the wall
        if (!(wallLeft && horizontalInput > 0) &&!(wallRight && horizontalInput < 0))
        {
            movement += -wallNormal * wallRunForce;
        }

        controller.Move(movement * Time.deltaTime);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight? rightWallhit.normal : leftWallhit.normal;

        Vector3 horizontalJump = wallNormal * wallJumpSideForce;

        // Vertical part is handled by TestePlayerCC
        pm.SetVerticalVelocity(wallJumpUpForce);

        // Move sideways
        controller.Move(horizontalJump * Time.deltaTime);

        StopWallRun();
    }
}
