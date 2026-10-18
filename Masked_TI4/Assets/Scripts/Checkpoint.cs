using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerRespawn respawn))
        {
            respawn.SetCheckpoint(transform);
        }
    }
}