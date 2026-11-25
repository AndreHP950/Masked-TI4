using UnityEngine;

public class ElevadorChegouNivel2 : MonoBehaviour
{
    public Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Teleportou", true);

        }
    }
}
