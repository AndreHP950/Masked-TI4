using UnityEngine;

public class ElevadorTriggerEntrou1 : MonoBehaviour
{
    public Animator animator;
    public Transform spawnpont3;
    public float timer;


    private void Update()
    {
        if (animator.GetBool("Entrou") == true)
        {
            timer += Time.deltaTime;
        }
        if(timer > 3f)
        {
            PlayerRespawn.instance.RespawnPlayer();
            timer = 0;
            animator.SetBool("Entrou", false);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn.instance.SetCheckpoint(spawnpont3);
            animator.SetBool("Entrou", true);

        }
    }
}
