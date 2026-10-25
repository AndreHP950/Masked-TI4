using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float limiteY = -10f;
    [SerializeField] private Transform pontoRespawn; 

    void Update()
    {
        if (transform.position.y <= limiteY)
        {
            Respawn();
        }
    }

    public void SetCheckpoint(Transform novoPonto)
    {
        pontoRespawn = novoPonto;
    }

    private void Respawn()
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = pontoRespawn.position;

        if (cc != null) cc.enabled = true;
    }

    public void RespawnPlayer()
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
}