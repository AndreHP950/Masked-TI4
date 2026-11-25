using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TurretProjectile : MonoBehaviour
{
    public float lifeTime = 5f;

    private Rigidbody rb;
    private Collider[] myColliders;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        myColliders = GetComponentsInChildren<Collider>();
    }

    public void Launch(Vector3 velocity, Collider[] ignore)
    {
        if (ignore != null)
        {
            foreach (Collider other in ignore)
            {
                if (other == null) continue;
                foreach (Collider mine in myColliders)
                    Physics.IgnoreCollision(mine, other);
            }
        }

        rb.AddForce(velocity, ForceMode.VelocityChange);
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponentInParent<TestePlayerCC>() != null)
        {
            PlayerRespawn.instance.RespawnPlayer();
        }

        Destroy(gameObject);
    }
}