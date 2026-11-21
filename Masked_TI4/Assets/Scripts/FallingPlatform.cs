using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f;
    public Rigidbody rb;
    public Animator animator;

    private bool triggered;

    void Awake()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger com: " + other.name + " | tag: " + other.tag);

        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        animator.SetTrigger("Shake");
        yield return new WaitForSeconds(fallDelay);

        animator.enabled = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}