using Unity.VisualScripting;
using UnityEngine;

public class PlayerMudarTempo : MonoBehaviour
{
    int tempo = 1;//1 = passado , 2 = futuro
    public bool canSwapTime = true;

    void Update()
    {
        if (canSwapTime)
        {
            if (Input.GetMouseButtonDown(button: 1))
            {
                if (tempo == 1)
                {
                    TrocarTempo.Instanciate.IrParaFuturo();
                    tempo = 2;
                }
                else
                {
                    TrocarTempo.Instanciate.IrParaPassado();
                    tempo = 1;
                }

            }
        }

        else
        {
            Debug.Log("O jogador esta na area de negacao de tempo.");
        }
    }
}
