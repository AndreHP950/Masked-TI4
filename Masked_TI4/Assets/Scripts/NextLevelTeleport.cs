
using UnityEngine;

public class NextLevelTeleport : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject player;
    public GameObject proximoNivel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (proximoNivel == null)
            {
                Debug.LogWarning("O destino do teleporte não foi definido!");
                return;
            }

            PlayerRespawn.instance.AtualizarRespawn(proximoNivel);

            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
                cc.enabled = false;

            other.transform.position = proximoNivel.transform.position;

            if (cc != null)
                cc.enabled = true;
        }
    }
}
