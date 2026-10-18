using UnityEngine;

public class uiController : MonoBehaviour
{

    public static uiController Instance;



    private void Awake()
    {
        // a logica ainda eh o que o roque falou na aula
        // mas n tenho exata certeza se tem como otimizar mais?
        // Daria par eu soh dar um instance = this direto no awake mas assim ao mnos garante que n vai ter mais de um

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        else

        {
            Instance = this;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Ainda n tem nada aqui
    }
}