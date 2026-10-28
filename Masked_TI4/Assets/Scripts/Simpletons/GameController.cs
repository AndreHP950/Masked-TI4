using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject Player;
    public UniversalRendererData data;
    public Material Correr;
    public PlayerMudarTempo timeControl;

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

    public void disableTravel()
    {
       timeControl.canSwapTime = false;
    }

    public void enableTravel()
    {
        timeControl.canSwapTime = true;
    }
}
