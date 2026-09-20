using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject Player;

    private void Awake()
    {
        

        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        else

        {
            instance = this;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void LivrarCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true ;
    }

    public void TravarCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
