using UnityEngine;

public class ElevadorTriggerNivel1 : MonoBehaviour
{
    public Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Chegou", true);
            
        }
    }
}
