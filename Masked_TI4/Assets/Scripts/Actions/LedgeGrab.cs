using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class LedgeGrab : MonoBehaviour
{
    [Header("Ledge Detection")]
    public Transform ledgeRayOrigin;
    public float wallDistance = 1f;
    public float topHeight = 1.5f;
    public float grabDistance = 0.4f;
    public LayerMask ledgeLayer;

    [Header("Climbing")]
    public float climbHeight = 1f;
    public float climbDuration = 0.5f;

    private CharacterController controller;
    private bool grabbing;
    private bool climbing;
    private Vector3 ledgeTopPosition;

    public bool IsGrabbing => grabbing;
   


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!grabbing && !climbing) CheckForLedge();
    }

    private void CheckForLedge()
    {
        if (ledgeRayOrigin == null) return;

        if (!Physics.Raycast(ledgeRayOrigin.position, transform.forward, out RaycastHit wallHit, wallDistance, ledgeLayer)) return;

        Vector3 topRayStart = wallHit.point + Vector3.up * topHeight;

        if (!Physics.Raycast(topRayStart, Vector3.down, out RaycastHit topHit, topHeight + 1f, ledgeLayer)) return;

        if (Vector3.Angle(topHit.normal, Vector3.up) > 45f) return;

        float heightDifference = topHit.point.y - transform.position.y;

        if (heightDifference < 0.2f || heightDifference > topHeight + 0.5f) return;

        
        float safeDistance = controller.radius + 0.05f;

        Vector3 position = transform.position;
        Vector3 wallNormal = wallHit.normal;
        wallNormal.y = 0f;
        wallNormal.Normalize();

        position += wallNormal * safeDistance;

        
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;

        ledgeTopPosition = topHit.point;
        grabbing = true;

        
    }

    public void ClimbLedge(InputAction.CallbackContext context)//Novo imput do player
    {
        if (!context.performed || !grabbing || climbing) return;
        StartCoroutine(ClimbRoutine());
    }

    private IEnumerator ClimbRoutine()//isso daqui e uma corrotina que coloca o player no topo do objeto;
    {
        climbing = true;
        grabbing = false;
        controller.enabled = false;

        Vector3 startPosition = transform.position;

        
        float playerHeight = GetComponent<CharacterController>().height;
        Vector3 targetPosition = ledgeTopPosition + transform.forward * 0.6f + Vector3.up * (playerHeight * 0.5f + 0.05f);

        float elapsed = 0f;

        while (elapsed < climbDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / climbDuration);
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        controller.enabled = true;
        climbing = false;

       
    }

    public void DropLedge(InputAction.CallbackContext context)
    {
        if (!context.performed || !grabbing || climbing) return;
        grabbing = false;
    }

    private void OnDrawGizmos()//ver o raycast e a distancia que estou calculando
    {
        if (ledgeRayOrigin == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(ledgeRayOrigin.position, transform.forward * wallDistance);
    }
}