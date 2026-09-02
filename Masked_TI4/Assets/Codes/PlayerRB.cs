using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRB : MonoBehaviour
{
    public float speed, jumpforce;
    float segurarSpeed;
    Rigidbody rb;
    Vector3 mov;
    bool Floored = false;
    int jumpCount;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        segurarSpeed = speed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 localDirection = new Vector3(x, 0f, z);
        Vector3 worldDirection = transform.TransformDirection(localDirection);


        Vector3 velocity = rb.linearVelocity;


        velocity.x = worldDirection.x * speed;
        velocity.z = worldDirection.z * speed;


        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            Floored = false;
            jumpCount++;
            velocity.y = jumpforce;
        }
        if(jumpCount >= 2 && Floored)
        {
            jumpCount = 0;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 2 * segurarSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = segurarSpeed;
        }

        rb.linearVelocity = velocity;
        print("Duble Jump:"+ jumpCount);
        print("Florred" + Floored);
    }

    void LateUpdate()
    {
        Vector3 camDir = Camera.main.transform.forward;
        camDir.y = 0;
        transform.rotation = Quaternion.LookRotation(camDir);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject)// depois ver se coloca tag
        {
            Floored = true;
            jumpCount = 0;
            
        }
    }
}
