using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NovoMov : MonoBehaviour
{
    CharacterController cc;
    Vector3 dir;
    public float speed = 6;
    public float JumpForce = 10;
    public float rotationSpeed = 0.2f;
    public float FallingSpeed = 0.5f;
    Transform cam;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        dir = new Vector3();
        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
       
        Gravity();
        cc.Move(dir * Time.deltaTime);

        Vector3 camDir = Camera.main.transform.forward;
        camDir.y = 0f;

        if (camDir.sqrMagnitude > 0.001f)
        {
            float targetY = Quaternion.LookRotation(camDir).eulerAngles.y;

            Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

    }
    private void LateUpdate()
    {
        Vector3 euler = transform.rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(
            0f,      // Freeze X
            euler.y, // Allow Y
            0f       // Freeze Z
        );
    }
    void FixedUpdate()
    {
        this.gameObject.transform.rotation = Quaternion.LookRotation(cam.forward);
    }

    void Gravity()
    {
        dir.y += (cc.isGrounded)? FallingSpeed * Time.deltaTime: -9.81f*Time.deltaTime;
        
        
    }
    public void Move(InputAction.CallbackContext cx)
    {
        Vector2 input = cx.ReadValue<Vector2>();
        dir = cam.right * input.x + cam.forward * input.y;
        dir = dir.normalized * speed ;
    }

    public void Jump(InputAction.CallbackContext cx)
    {
        if (cc.isGrounded)
        {
            dir.y = JumpForce;
        }
        
    }
}
