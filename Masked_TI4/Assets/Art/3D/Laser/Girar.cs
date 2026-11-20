using UnityEngine;

public class Girar : MonoBehaviour
{
    public float velocidade = 100f;
    public Vector3 eixo = Vector3.up;
    bool girar = false;
    public GameObject objeto;
    
    void Update()
    {
        if(girar) objeto.transform.Rotate(0f, velocidade * Time.deltaTime, 0f);
    }

    void GirarObj()
    {
        girar = true;
    }
}