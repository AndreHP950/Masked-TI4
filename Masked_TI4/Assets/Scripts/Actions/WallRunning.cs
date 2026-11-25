using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class WallRunning : MonoBehaviour
{
    private enum WallState { None, Running, Switching }

    [Header("Wall Detection")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallCheckDistance = 1f;
    public float minJumpHeight = 1f;
    [Range(0f, 1f)] public float maxWallNormalY = 0.3f;
    public float minWallLengthAhead = 1.5f;

    [Header("Wall Running")]
    public float wallRunSpeed = 12f;
    public float wallClimbSpeed = 5f;
    public float maxWallRunTime = 0f;
    public float wallSkin = 0.1f;
    public float stickStrength = 25f;

    [Header("Troca de Parede (A / D)")]
    public float wallSwitchDistance = 6f;
    public float wallSwitchSpeed = 15f;
    public float wallSwitchMaxTime = 0.6f;
    public float sideInputBuffer = 0.15f;

    [Header("Wall Exit / Jump")]
    public float exitWallTime = 0.5f;
    public float wallJumpUpForce = 5f;
    public float wallJumpSideForce = 14f;
    [Range(0f, 1f)] public float jumpForwardCarry = 0.8f;
    public float jumpAirDrag = 1.2f;

    [Header("References")]
    public Transform orientation;

    private TestePlayerCC pm;
    private CharacterController controller;

    private WallState state = WallState.None;
    private Vector3 wallNormal;
    private int wallSide;
    private Vector3 runDir;
    private float currentRunSpeed;

    private Vector3 switchNormal;
    private int switchSide;
    private float switchTimer;

    private float wallRunTimer;
    private float exitWallTimer;
    private bool jumpedIntoAir;
    private float jumpGraceTimer;
    private bool pendingWallJump;

    private Vector3 externalVelocity;

    private bool upwardsRunning, downwardsRunning;
    private float horizontalInput;
    private int pendingSide;
    private float pendingSideTimer;

    private Vector3 Center => transform.TransformPoint(controller.center);
    private float StickDistance => controller.radius + wallSkin;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pm = GetComponent<TestePlayerCC>();
        wallRunTimer = maxWallRunTime;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (exitWallTimer > 0f) exitWallTimer -= dt;
        if (state == WallState.None && controller.isGrounded) wallRunTimer = maxWallRunTime;

        if (pendingWallJump)
        {
            pendingWallJump = false;
            if (state == WallState.Running) WallJump();
        }
        if (jumpGraceTimer > 0f) jumpGraceTimer -= dt;
        else if (controller.isGrounded) jumpedIntoAir = false;

        HandleSideInput(dt);

        switch (state)
        {
            case WallState.None: TryStartWallRun(); break;
            case WallState.Running: WallRunUpdate(dt); break;
            case WallState.Switching: SwitchUpdate(dt); break;
        }

        ApplyExternalVelocity(dt);
        UpdateAnimation();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > 0.5f && Mathf.Abs(horizontalInput) <= 0.5f)
        {
            pendingSide = input.x > 0f ? 1 : -1;
            pendingSideTimer = sideInputBuffer;
        }

        horizontalInput = input.x;
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
        if (!ctx.performed) return;

        if (state == WallState.Running)
        {
            pendingWallJump = true;
        }
        else if (state == WallState.None && (controller.isGrounded || !AboveGround()))
        {
            MarkJumped();
        }
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private bool IsRunnableWall(RaycastHit hit, Vector3 preferredForward, out Vector3 forward)
    {
        forward = Vector3.zero;

        if (Mathf.Abs(hit.normal.y) > maxWallNormalY) return false;

        forward = Vector3.Cross(hit.normal, Vector3.up).normalized;
        Vector3 pref = Vector3.ProjectOnPlane(preferredForward, Vector3.up);
        if (Vector3.Dot(forward, pref) < 0f) forward = -forward;

        if (minWallLengthAhead > 0f)
        {
            Vector3 aheadOrigin = hit.point + hit.normal * StickDistance + forward * minWallLengthAhead;
            aheadOrigin.y = Center.y;
            if (!Physics.Raycast(aheadOrigin, -hit.normal, StickDistance + 0.5f, whatIsWall))
                return false;
        }

        return true;
    }

    private void TryStartWallRun()
    {
        if (!jumpedIntoAir || exitWallTimer > 0f || !AboveGround()) return;

        float range = controller.radius + wallCheckDistance;
        Vector3 fwd = orientation.forward;

        if (Physics.Raycast(Center, orientation.right, out RaycastHit rightHit, range, whatIsWall) &&
            IsRunnableWall(rightHit, fwd, out Vector3 rightFwd))
        {
            StartWallRun(rightHit, 1, rightFwd);
        }
        else if (Physics.Raycast(Center, -orientation.right, out RaycastHit leftHit, range, whatIsWall) &&
                 IsRunnableWall(leftHit, fwd, out Vector3 leftFwd))
        {
            StartWallRun(leftHit, -1, leftFwd);
        }
    }

    private void StartWallRun(RaycastHit hit, int side, Vector3 forward)
    {
        if (state == WallState.None)
        {
            Vector3 hv = controller.velocity;
            hv.y = 0f;
            currentRunSpeed = Mathf.Max(wallRunSpeed, Vector3.Dot(hv, forward));
        }

        state = WallState.Running;
        pm.wallrunning = true;
        wallNormal = hit.normal;
        wallSide = side;
        runDir = forward;
        externalVelocity = Vector3.zero;
        pm.SetVerticalVelocity(0f);
    }

    private void WallRunUpdate(float dt)
    {
        if (!Physics.Raycast(Center, -wallNormal, out RaycastHit hit, controller.radius + wallCheckDistance, whatIsWall) ||
            Mathf.Abs(hit.normal.y) > maxWallNormalY)
        {
            StopWallRun();
            return;
        }

        if (!AboveGround())
        {
            StopWallRun();
            return;
        }

        if (maxWallRunTime > 0f)
        {
            wallRunTimer -= dt;
            if (wallRunTimer <= 0f)
            {
                StopWallRun();
                exitWallTimer = exitWallTime;
                jumpedIntoAir = false;
                externalVelocity = wallNormal * 2f;
                return;
            }
        }

        pm.SetVerticalVelocity(0f);

        wallNormal = hit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up).normalized;
        if (Vector3.Dot(wallForward, runDir) < 0f) wallForward = -wallForward;
        runDir = wallForward;

        Vector3 velocity = runDir * currentRunSpeed;

        velocity.y = upwardsRunning ? wallClimbSpeed : downwardsRunning ? -wallClimbSpeed : 0f;

        Vector3 displacement = velocity * dt;

        float error = hit.distance - StickDistance;
        displacement += -wallNormal * error * Mathf.Clamp01(stickStrength * dt);

        controller.Move(displacement);
    }

    private void SwitchUpdate(float dt)
    {
        switchTimer -= dt;

        if (switchTimer <= 0f || !AboveGround() ||
            !Physics.Raycast(Center, -switchNormal, out RaycastHit hit, controller.radius + wallSwitchDistance, whatIsWall))
        {
            CancelSwitch();
            return;
        }

        pm.SetVerticalVelocity(0f);

        float remaining = hit.distance - StickDistance;
        if (remaining <= 0.05f)
        {
            StartWallRun(hit, switchSide, runDir);
            return;
        }

        float lateralStep = Mathf.Min(wallSwitchSpeed * dt, remaining);
        Vector3 displacement = runDir * currentRunSpeed * dt + (-switchNormal) * lateralStep;

        CollisionFlags flags = controller.Move(displacement);

        if ((flags & CollisionFlags.Sides) != 0 &&
            Physics.Raycast(Center, -switchNormal, out RaycastHit arrived, StickDistance + 0.2f, whatIsWall))
        {
            StartWallRun(arrived, switchSide, runDir);
        }
    }

    private void HandleSideInput(float dt)
    {
        if (pendingSide == 0) return;

        if (exitWallTimer > 0f)
        {
            pendingSide = 0;
            return;
        }

        pendingSideTimer -= dt;
        if (pendingSideTimer <= 0f)
        {
            pendingSide = 0;
            return;
        }

        if (state == WallState.Switching) return;

        Vector3 searchDir;
        Vector3 preferredForward;

        if (state == WallState.Running)
        {
            if (pendingSide == wallSide) { pendingSide = 0; return; }

            searchDir = wallNormal;
            preferredForward = runDir;
        }
        else
        {
            if (!jumpedIntoAir || !AboveGround()) return;
            searchDir = pendingSide > 0 ? orientation.right : -orientation.right;
            preferredForward = orientation.forward;
        }

        searchDir = Vector3.ProjectOnPlane(searchDir, Vector3.up).normalized;

        if (!Physics.Raycast(Center, searchDir, out RaycastHit hit, controller.radius + wallSwitchDistance, whatIsWall))
            return;

        if (Vector3.Dot(hit.normal, -searchDir) < 0.5f) return;
        if (!IsRunnableWall(hit, preferredForward, out Vector3 newForward)) return;

        BeginSwitch(hit, pendingSide, newForward);
        pendingSide = 0;
    }

    private void BeginSwitch(RaycastHit hit, int side, Vector3 forward)
    {
        state = WallState.Switching;
        pm.wallrunning = true;
        switchNormal = hit.normal;
        switchSide = side;
        runDir = forward;
        switchTimer = wallSwitchMaxTime;
        externalVelocity = Vector3.zero;
        pm.SetVerticalVelocity(0f);
    }

    private void CancelSwitch()
    {
        state = WallState.None;
        pm.wallrunning = false;
    }

    private void StopWallRun()
    {
        state = WallState.None;
        pm.wallrunning = false;
    }

    private void WallJump()
    {
        Vector3 normal = Vector3.ProjectOnPlane(wallNormal, Vector3.up).normalized;
        Vector3 forward = runDir;

        StopWallRun();
        exitWallTimer = exitWallTime;
        MarkJumped();

        pm.SetVerticalVelocity(wallJumpUpForce);
        AnimationController.instance.anim.SetBool("Jump", true);

        externalVelocity = normal * wallJumpSideForce + forward * currentRunSpeed * jumpForwardCarry;
    }

    private void MarkJumped()
    {
        jumpedIntoAir = true;
        jumpGraceTimer = 0.2f;
    }

    private void ApplyExternalVelocity(float dt)
    {
        if (state != WallState.None)
        {
            externalVelocity = Vector3.zero;
            return;
        }
        if (externalVelocity.sqrMagnitude < 0.01f) return;

        CollisionFlags flags = controller.Move(externalVelocity * dt);
        externalVelocity *= Mathf.Exp(-jumpAirDrag * dt);

        if ((flags & CollisionFlags.Below) != 0 || (flags & CollisionFlags.Sides) != 0)
            externalVelocity = Vector3.zero;
    }

    private void UpdateAnimation()
    {
        Animator anim = AnimationController.instance.anim;
        bool onWall = state != WallState.None;

        if (onWall)
        {
            int side = state == WallState.Switching ? switchSide : wallSide;
            anim.SetInteger("Pos", side < 0 ? 1 : 2);
        }
        if (onWall) anim.SetBool("Jump", false);
        anim.SetBool("Wallrunning", onWall);
    }

    private void OnDrawGizmosSelected()
    {
        if (orientation == null) return;
        CharacterController cc = controller != null ? controller : GetComponent<CharacterController>();
        if (cc == null) return;

        Vector3 c = transform.TransformPoint(cc.center);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(c, orientation.right * (cc.radius + wallCheckDistance));
        Gizmos.DrawRay(c, -orientation.right * (cc.radius + wallCheckDistance));
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(c + Vector3.up * 0.1f, orientation.right * (cc.radius + wallSwitchDistance));
        Gizmos.DrawRay(c + Vector3.up * 0.1f, -orientation.right * (cc.radius + wallSwitchDistance));
    }
}