using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float limiteY = -10f;
    [SerializeField] private Transform pontoRespawn;
    public static PlayerRespawn instance;


    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (transform.position.y <= limiteY)
        {
            Morrer();
        }
    }

    public void Morrer()
    {
        Debug.Log("Morreu");
        if (GameOverController.instance != null)
        {
            GameOverController.instance.TriggerGameOver();
        }
        else
        {
            RespawnPlayer();
        }
    }

    public void SetCheckpoint(Transform novoPonto)
    {
        pontoRespawn = novoPonto;
    }

    public void Respawn()
    {
        if (pontoRespawn == null)
            return;

        CharacterController cc = GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;
        foreach (GasZoneTrigger zone in FindObjectsOfType<GasZoneTrigger>())
            zone.ResetGasEffect();

        transform.position = pontoRespawn.position;

        if (cc != null)
            cc.enabled = true;
    }

    public void RespawnPlayer()
    {
        GameOverController.instance.TriggerGameOver();

    }


    public void AtualizarRespawn(GameObject proximoRes)
    {
        pontoRespawn = proximoRes.transform;
    }
}
