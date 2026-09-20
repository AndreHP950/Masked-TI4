using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    public static SceneManagerController instance;
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadSceneByIndex(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadSceneByIndex(2);
        }
    }

    
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
