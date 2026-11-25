using UnityEngine;

public class TurretTrap : MonoBehaviour
{
    [Header("Detection")]
    public Transform player;
    public float viewAngle = 60f;
    public float range = 10f;
    public float loseTargetRange = 15f;

    [Header("Aim")]
    public Transform pivot;
    public float turnSpeed = 180f;
    public bool onlyHorizontal = false;
    public float aimTolerance = 5f;
    public bool returnToRest = true;

    [Header("Shooting")]
    public TurretProjectile projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireRate = 1f;
    public float firstShotDelay = 0.5f;

    [Header("Visual")]
    public Renderer turretRenderer;
    public Color alertColor = Color.red;

    private Color originalColor;
    private CharacterController playerController;
    private Collider[] ownColliders;
    private Quaternion restRotation;
    private bool locked;
    private float fireTimer;

    private Transform Pivot => pivot != null ? pivot : transform;

    void Start()
    {
        if (turretRenderer != null) originalColor = turretRenderer.material.color;
        if (player != null) playerController = player.GetComponent<CharacterController>();
        ownColliders = GetComponentsInChildren<Collider>();
        restRotation = Pivot.rotation;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 target = TargetPoint();

        if (!locked)
        {
            if (CanSeePlayer())
            {
                locked = true;
                fireTimer = firstShotDelay;
            }
        }
        else if ((target - Pivot.position).sqrMagnitude > loseTargetRange * loseTargetRange)
        {
            locked = false;
        }

        if (turretRenderer != null)
            turretRenderer.material.color = locked ? alertColor : originalColor;

        if (!locked)
        {
            if (returnToRest)
                Pivot.rotation = Quaternion.RotateTowards(Pivot.rotation, restRotation, turnSpeed * Time.deltaTime);
            return;
        }

        AimAt(target);

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f && IsAimed(target))
        {
            Shoot(target);
            fireTimer = fireRate > 0f ? 1f / fireRate : 1f;
        }
    }

    Vector3 TargetPoint()
    {
        if (playerController != null) return player.TransformPoint(playerController.center);
        return player.position;
    }

    void AimAt(Vector3 target)
    {
        Vector3 dir = target - Pivot.position;
        if (onlyHorizontal) dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion look = Quaternion.LookRotation(dir);
        Pivot.rotation = Quaternion.RotateTowards(Pivot.rotation, look, turnSpeed * Time.deltaTime);
    }

    bool IsAimed(Vector3 target)
    {
        Vector3 dir = target - Pivot.position;
        if (onlyHorizontal) dir.y = 0f;
        return Vector3.Angle(Pivot.forward, dir) <= aimTolerance;
    }

    void Shoot(Vector3 target)
    {
        if (projectilePrefab == null) return;

        Transform origin = firePoint != null ? firePoint : Pivot;
        Vector3 dir = (target - origin.position).normalized;

        TurretProjectile projectile = Instantiate(projectilePrefab, origin.position, Quaternion.LookRotation(dir));
        projectile.Launch(dir * projectileSpeed, ownColliders);
    }

    bool CanSeePlayer()
    {
        Vector3 toPlayer = TargetPoint() - Pivot.position;
        if (toPlayer.sqrMagnitude > range * range) return false;

        float dot = Vector3.Dot(Pivot.forward, toPlayer.normalized);
        float threshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
        return dot > threshold;
    }

    void OnDrawGizmos()
    {
        Transform p = Pivot;
        Vector3 origin = p.position;
        float halfAngle = viewAngle * 0.5f;

        Vector3 leftDir = Quaternion.AngleAxis(-halfAngle, p.up) * p.forward;
        Vector3 rightDir = Quaternion.AngleAxis(halfAngle, p.up) * p.forward;

        bool playerVisible = locked || (player != null && CanSeePlayer());
        Gizmos.color = playerVisible ? Color.red : Color.yellow;

        Gizmos.DrawLine(origin, origin + leftDir * range);
        Gizmos.DrawLine(origin, origin + rightDir * range);

        int segments = 20;
        Vector3 previous = origin + leftDir * range;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + viewAngle * i / segments;
            Vector3 dir = Quaternion.AngleAxis(angle, p.up) * p.forward;
            Vector3 next = origin + dir * range;
            Gizmos.DrawLine(previous, next);
            previous = next;
        }

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(origin, loseTargetRange);
    }
}