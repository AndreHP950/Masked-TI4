using UnityEngine;

public class TurretTrap : MonoBehaviour
{
    public Transform player;
    public float viewAngle = 60f;
    public float range = 10f;

    public Renderer turretRenderer;
    public Color alertColor = Color.red;

    Color originalColor;

    void Start()
    {
        originalColor = turretRenderer.material.color;
    }

    void Update()
    {
        bool playerVisible = CanSeePlayer();
        turretRenderer.material.color = playerVisible ? alertColor : originalColor;

        if (playerVisible)
        {
            
        }
    }

    bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.sqrMagnitude > range * range) return false;

        float dot = Vector3.Dot(transform.forward, toPlayer.normalized);
        float threshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
        return dot > threshold;
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        float halfAngle = viewAngle * 0.5f;

        Vector3 leftDir = Quaternion.AngleAxis(-halfAngle, transform.up) * transform.forward;
        Vector3 rightDir = Quaternion.AngleAxis(halfAngle, transform.up) * transform.forward;

        bool playerVisible = player != null && CanSeePlayer();
        Gizmos.color = playerVisible ? Color.red : Color.yellow;

        Gizmos.DrawLine(origin, origin + leftDir * range);
        Gizmos.DrawLine(origin, origin + rightDir * range);

        
        int segments = 20;
        Vector3 previous = origin + leftDir * range;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + viewAngle * i / segments;
            Vector3 dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            Vector3 next = origin + dir * range;
            Gizmos.DrawLine(previous, next);
            previous = next;
        }
    }

}