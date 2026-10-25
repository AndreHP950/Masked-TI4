
using UnityEngine;
using System.Collections;

public class TriggerTraps : MonoBehaviour
{
    [SerializeField] private float tempoParaMorrer = 2f;

    private Coroutine gasCoroutine;
    private PlayerRespawn playerRespawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gas"))
        {
            playerRespawn = GetComponent<PlayerRespawn>();

            if (playerRespawn == null)
                playerRespawn = GetComponentInParent<PlayerRespawn>();

            if (playerRespawn != null)
                gasCoroutine = StartCoroutine(EsperarGas());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gas"))
        {
            if (gasCoroutine != null)
            {
                StopCoroutine(gasCoroutine);
                gasCoroutine = null;
            }
        }
    }

    private IEnumerator EsperarGas()
    {
        yield return new WaitForSeconds(tempoParaMorrer);

        if (playerRespawn != null)
        {
            playerRespawn.RespawnPlayer();
        }

        gasCoroutine = null;
    }
}
