using UnityEngine;

public class TrocarTempo : MonoBehaviour
{
    public static TrocarTempo Instanciate;
    public GameObject passado;
    public GameObject futuro;


    private void Start()
    {
        Instanciate = this;
    }
    public void IrParaPassado()
    {
        passado.SetActive(true);
        futuro.SetActive(false);
    }

    public void IrParaFuturo()
    {
        passado.SetActive(false);
        futuro.SetActive(true);
    }
}