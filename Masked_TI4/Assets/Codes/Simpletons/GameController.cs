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
}
