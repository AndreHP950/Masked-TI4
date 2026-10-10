using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Detection")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallCheckDistance = 1f;
    public float minJumpHeight = 1f;

    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft, wallRight;

    [Header("Wall Running")]
    public float wallRunForce = 10f;
    public float wallClimbSpeed = 5f;
    public float maxWallRunTime = 3f;
    private float wallRunTimer;
    private bool upwardsRunning, downwardsRunning;
    private float horizontalInput, verticalInput;

    [Header("Wall Exit")]
    public float exitWallTime = 0.2f;
    private bool exitingWall;
    private float exitWallTimer;

    [Header("Wall Jump")]
    public float wallJumpUpForce = 8f;
    public float wallJumpSideForce = 8f;

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
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        horizontalInput = input.x;
        verticalInput = input.y;
    }

    public void WallRunUp(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) upwardsRunning = true;
        else if (ctx.canceled) upwardsRunning = false;
    }

    public void WallRunDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) downwardsRunning = true;
        else if (ctx.canceled) downwardsRunning = false;
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && pm.wallrunning) WallJump();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning) StartWallRun();
            if (wallRunTimer > 0) wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
        }
        else if (exitingWall)
        {
            if (pm.wallrunning) StopWallRun();
            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0) exitingWall = false;
        }
        else
        {
            if (pm.wallrunning) StopWallRun();
        }

        if (pm.wallrunning) WallRunningMovement();
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(orientation.forward, wallForward) < 0f) wallForward = -wallForward;

        Vector3 movement = wallForward * wallRunForce;
        float verticalMovement = upwardsRunning ? wallClimbSpeed : downwardsRunning ? -wallClimbSpeed : 0f;
        movement.y = verticalMovement;

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            movement += -wallNormal * wallRunForce;

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

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 horizontalJump = wallNormal * wallJumpSideForce;

        pm.SetVerticalVelocity(wallJumpUpForce);
        controller.Move(horizontalJump * Time.deltaTime);
        StopWallRun();
    }
}