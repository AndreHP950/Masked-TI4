using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRB : MonoBehaviour
{
    public float speed, jumpforce;
    public bool wallrunning;
    float segurarSpeed;
    Rigidbody rb;
    Vector3 mov;
    bool Floored = false;
    int jumpCount;
    float x;
    float z;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        segurarSpeed = speed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (wallrunning) return;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");


        
        if (Input.GetKeyDown(KeyCode.W))
        {
            AnimationController.instance.ControlarWalk(1);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            AnimationController.instance.ControlarWalk(0);
        }


        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 2f * segurarSpeed;
        }
        else
        {
            speed = segurarSpeed;
        }


       
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            Floored = false;

            jumpCount++;

           
            Vector3 velocity = rb.linearVelocity;
            velocity.y = jumpforce;

            rb.linearVelocity = velocity;

            AudioManager.instance.PlaySFX(1);
            AnimationController.instance.ControlarJump(1);

            Debug.Log("Jump: " + jumpCount);
        }

    }

    void FixedUpdate()
    {
        
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();


       
        Vector3 dir = (right * x + forward * z) * speed;


        
        Vector3 velocity = rb.linearVelocity;

        velocity.x = dir.x;
        velocity.z = dir.z;

       
        rb.linearVelocity = velocity;


        
        Vector3 camDir = Camera.main.transform.forward;
        camDir.y = 0f;

        if (camDir.sqrMagnitude > 0.001f)
        {
            float yRotation = Quaternion.LookRotation(camDir).eulerAngles.y;

            Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);

            rb.MoveRotation(targetRotation);
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject)// depois ver se coloca tag
        {
            Floored = true;
            jumpCount = 0;
            AnimationController.instance.ControlarJump(0);
            
        }
    }

    private void OnTriggerEnter(Collider other) // Para teste de audio. Pode remover depois se for o caso
    {
        if (other.gameObject.tag == "Coin")
        {
            AudioManager.instance.PlaySFX(2);
            Destroy(other.gameObject);
        }
    }
}
