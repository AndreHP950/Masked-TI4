using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class WaypointMover : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Pontos do percurso. Posições relativas à posição inicial do objeto em cena.")]
    public List<Vector3> waypoints = new List<Vector3>();

    [Header("Movimento")]
    [Tooltip("Velocidade de movimento do objeto.")]
    public float speed = 2f;
    [Tooltip("Distância para considerar que chegou no waypoint.")]
    public float arrivalDistance = 0.05f;

    [Header("Rotação")]
    [Tooltip("Velocidade de rotação em graus por segundo.")]
    public float rotationSpeed = 180f;
    [Tooltip("Diferença de ângulo (graus) para considerar que está alinhado e pode andar.")]
    public float alignTolerance = 2f;
    [Tooltip("Ajuste de rotação aplicado sobre a direção do movimento. Use para corrigir o eixo do modelo (ex.: -90 em X).")]
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Área de parada (Trigger)")]
    [Tooltip("Centro local da área cúbica. Quando o player entrar nela, o objeto para.")]
    public Vector3 stopAreaCenter = Vector3.zero;
    [Tooltip("Tamanho da área cúbica.")]
    public Vector3 stopAreaSize = new Vector3(3f, 3f, 3f);

    [Header("Debug")]
    [Tooltip("Mostra logs no Console para diagnosticar o movimento.")]
    public bool debugLogs = false;
    public bool showGizmos = true;
    public Color waypointColor = Color.cyan;
    public Color pathColor = Color.yellow;
    public Color stopAreaColor = Color.red;

    private Rigidbody rb;
    private BoxCollider stopArea;
    private Vector3 startPosition;
    private List<Vector3> worldWaypoints = new List<Vector3>();
    private int currentIndex = 0;
    private bool stopped = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        stopArea = GetComponent<BoxCollider>();
        ApplyStopArea();

        startPosition = transform.position;
        ConvertWaypointsToWorld();

        if (debugLogs)
        {
            Debug.Log($"[WaypointMover] {name}: {worldWaypoints.Count} waypoints carregados.");
        }
    }

    void OnValidate()
    {
        stopArea = GetComponent<BoxCollider>();
        ApplyStopArea();
    }

    void FixedUpdate()
    {
        if (stopped || worldWaypoints.Count < 2) return;

        Vector3 target = worldWaypoints[currentIndex];
        Vector3 current = rb.position;
        Vector3 toTarget = target - current;

        // Chegou no waypoint atual? Avança para o próximo (loop no último)
        if (toTarget.sqrMagnitude <= arrivalDistance * arrivalDistance)
        {
            currentIndex = (currentIndex + 1) % worldWaypoints.Count;

            if (debugLogs)
            {
                Debug.Log($"[WaypointMover] {name}: indo para waypoint {currentIndex}.");
            }
            return;
        }

        // Direção horizontal até o alvo (ignora Y para a rotação)
        Vector3 flatDirection = toTarget;
        flatDirection.y = 0f;

        if (flatDirection.sqrMagnitude > 0.0001f)
        {
            // Rotação alvo = direção do movimento + ajuste do modelo
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection.normalized, Vector3.up)
                                        * Quaternion.Euler(rotationOffset);

            // Fase 1: gira até ficar alinhado com o próximo ponto
            if (Quaternion.Angle(rb.rotation, targetRotation) > alignTolerance)
            {
                Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(newRotation);
                return;
            }
        }

        // Fase 2: alinhado, então anda em linha reta até o waypoint
        Vector3 next = Vector3.MoveTowards(current, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stopped = true;

            if (debugLogs)
            {
                Debug.Log($"[WaypointMover] {name}: player entrou na área, parando.");
            }
        }
    }

    private void ApplyStopArea()
    {
        if (stopArea == null) return;

        stopArea.isTrigger = true;
        stopArea.center = stopAreaCenter;
        stopArea.size = stopAreaSize;
    }

    private void ConvertWaypointsToWorld()
    {
        worldWaypoints.Clear();

        foreach (Vector3 wp in waypoints)
        {
            worldWaypoints.Add(startPosition + wp);
        }
    }

    // ========== GIZMOS ==========

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Área de parada (acompanha a posição e a escala do objeto)
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(stopAreaColor.r, stopAreaColor.g, stopAreaColor.b, 0.2f);
        Gizmos.DrawCube(stopAreaCenter, stopAreaSize);
        Gizmos.color = stopAreaColor;
        Gizmos.DrawWireCube(stopAreaCenter, stopAreaSize);
        Gizmos.matrix = Matrix4x4.identity;
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        if (waypoints == null || waypoints.Count == 0) return;

        Vector3 basePos = Application.isPlaying ? startPosition : transform.position;

        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector3 worldPos = basePos + waypoints[i];

            Gizmos.color = waypointColor;
            Gizmos.DrawSphere(worldPos, 0.3f);

            if (i < waypoints.Count - 1)
            {
                Vector3 nextWorldPos = basePos + waypoints[i + 1];
                Gizmos.color = pathColor;
                Gizmos.DrawLine(worldPos, nextWorldPos);
            }
        }

        // Linha de fechamento do loop (último -> primeiro)
        if (waypoints.Count >= 2)
        {
            Gizmos.color = new Color(pathColor.r, pathColor.g, pathColor.b, 0.4f);
            Gizmos.DrawLine(basePos + waypoints[waypoints.Count - 1], basePos + waypoints[0]);
        }
    }
}