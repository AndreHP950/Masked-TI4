using UnityEngine;

public class ElevadorSaiuNivel2 : MonoBehaviour
{
    public Transform checkpoint;
    public Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn.instance.SetCheckpoint(checkpoint);
            animator.SetBool("Saiu", true);

        }
    }
}
