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

    private void Respawn()
    {
        transform.position = pontoRespawn.position;
    }
}
