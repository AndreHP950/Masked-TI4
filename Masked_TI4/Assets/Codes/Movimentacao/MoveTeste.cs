using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveTeste : MonoBehaviour
{
    Vector3 dir;
    CharacterController cc;
    public float speed;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
       
        /*if (dir.magnitude == 0) return;
        Vector3 rot = Camera.main.transform.forward;
        rot.y = 0;
        transform.rotation = Quaternion.LookRotation(rot);
        cc.SimpleMove(rot.normalized * dir.z * speed);
        print(dir);*/
    }
    void FixedUpdate()
    {
        //cc.Move(new Vector3(0, -9.81f, 0));
        cc.Move(dir * Time.fixedDeltaTime );
        print(dir);
    }

    public void Mover(InputAction.CallbackContext cx)
    {
        print(cx.ReadValue<Vector2>());
        Vector2 dirinput = cx.ReadValue<Vector2>();
        dir.x = dirinput.x;
        dir.z = dirinput.y;
        Vector2 input = cx.ReadValue<Vector2>();
        //dir = Vector3.right * input.x * speed + Vector3.forward * input.y * speed;//Global
        //dir = transform.right * input.x * speed + transform.forward * input.y * speed;//local
        /*Transform cam = Camera.main.transform;
        dir = cam.right * input.x + cam.forward * input.y;
        dir.y = 0;
        dir = dir.normalized * speed;*/ //Camera com o normalized colocado certo
    }
}
