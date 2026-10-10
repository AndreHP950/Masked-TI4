using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TestePlayerCC : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float sprintSpeed = 12f;
    public float acceleration = 20f;
    public float sprintAcceleration = 25f;
    public float deceleration = 30f;
    public float rotationSpeed = 10f;

    private float currentSpeed;
    private bool sprinting;

    [Header("Jump")]
    public float jumpforce = 8f;
    public int maxJumps = 2;

    private int jumpsRemaining;

    [Header("Gravity")]
    public float upGravity = -15f;
    public float downGravity = -30f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;

    private float coyoteTimer;
    private float verticalVelocity;

    [Header("State")]
    public bool wallrunning;

    private CharacterController controller;

    private float x;
    private float z;

    private bool Floored;

    public PlayerInput playerInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap("Player");

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (wallrunning) return;

        LedgeGrab ledge = GetComponent<LedgeGrab>();//lucas lima

        if (ledge != null && ledge.IsGrabbing)
        {
            verticalVelocity = 0f;
            return;
        }
        // =====================================
        // GROUND / GRAVITY
        // =====================================

        if (controller.isGrounded)
        {
            Floored = true;
            coyoteTimer = coyoteTime;
            AnimationController.instance.anim.SetBool("Jump",false);

            jumpsRemaining = maxJumps;

            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }
        else
        {
            Floored = false;
            coyoteTimer -= Time.deltaTime;

            if (verticalVelocity > 0f)
                verticalVelocity += upGravity * Time.deltaTime;
            else
                verticalVelocity += downGravity * Time.deltaTime;
        }

        // =====================================
        // CAMERA RELATIVE MOVEMENT
        // =====================================

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * x + forward * z;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        // =====================================
        // ACCELERATION / DECELERATION
        // =====================================

        float targetSpeed = direction.magnitude * (sprinting ? sprintSpeed : speed);

        if (direction.sqrMagnitude > 0.001f)
        {
            float accelerationRate = sprinting ? sprintAcceleration : acceleration;

            currentSpeed = Mathf.MoveTowards(currentSpeed,targetSpeed,accelerationRate * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // =====================================
        // CHARACTER MOVEMENT
        // =====================================

        Vector3 movement = direction.normalized * currentSpeed;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        // =====================================
        // STRAFE ROTATION
        // =====================================

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        if (cameraForward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

            transform.rotation = Quaternion.Slerp( transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // =========================================
    // MOVE INPUT
    // =========================================

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        x = input.x;
        z = input.y;
        AnimationController.instance.anim.SetInteger("WalkingSpeed", 1);
        if (context.canceled)
        {
            AnimationController.instance.anim.SetInteger("WalkingSpeed", 0);
            x = 0f;
            z = 0f;
        }
    }

    // =========================================
    // JUMP INPUT - DOUBLE JUMP
    // =========================================

    public void Jump(InputAction.CallbackContext context)
    {
        AnimationController.instance.anim.SetBool("Jump", true);
        if (!context.performed)
            return;

        // First jump
        if (jumpsRemaining == maxJumps && (controller.isGrounded || coyoteTimer > 0f))
        {
            verticalVelocity = jumpforce;
            jumpsRemaining--;
            coyoteTimer = 0f;

            AudioManager.instance.PlaySFX(1);
            return;
        }

        // Second jump
        if (jumpsRemaining > 0)
        {
            verticalVelocity = jumpforce;
            jumpsRemaining--;
           
            AudioManager.instance.PlaySFX(1);
        }
    }

    // =========================================
    // SPRINT INPUT
    // =========================================

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.started)
            sprinting = true;

        if (context.canceled)
            sprinting = false;
    }

    // =========================================
    // WALL JUMP SUPPORT
    // =========================================

    public void SetVerticalVelocity(float velocity)
    {
        verticalVelocity = velocity;
    }

    public float GetVerticalVelocity()
    {
        return verticalVelocity;
    }

    public void ResetCoyoteTime()
    {
        coyoteTimer = 0f;
    }

    // =========================================
    // TRIGGER
    // =========================================

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            AudioManager.instance.PlaySFX(2);
            Destroy(other.gameObject);
        }
    }
}
