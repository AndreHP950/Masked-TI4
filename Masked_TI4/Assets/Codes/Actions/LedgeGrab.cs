using UnityEngine;

public class LedgeGrab : MonoBehaviour
{
    public Transform ledgeRayOrigin;
    public float wallDistance = 1f;
    public float topHeight = 1.5f;
    public float grabDistance = 0.4f;
    public LayerMask ledgeLayer;

    private CharacterController controller;
    private bool grabbing;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!grabbing)
            CheckForLedge();
        else if (Input.GetKeyDown(KeyCode.Space))
            Drop();
    }

    private void CheckForLedge()
    {
        if (!Physics.Raycast(ledgeRayOrigin.position, transform.forward, out RaycastHit wallHit, wallDistance, ledgeLayer))
            return;

        Vector3 topRayStart = wallHit.point + Vector3.up * topHeight;

        if (!Physics.Raycast(topRayStart, Vector3.down, out RaycastHit topHit, topHeight + 1f, ledgeLayer))
            return;

        if (Vector3.Angle(topHit.normal, Vector3.up) > 45f)
            return;

        grabbing = true;

        Vector3 position = topHit.point;
        position += wallHit.normal * grabDistance;
        position.y -= 1f;

        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;

        Debug.Log("LEDGE FOUND!");
    }

    private void Drop()
    {
        grabbing = false;
    }

    private void OnDrawGizmos()
    {
        if (ledgeRayOrigin == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(ledgeRayOrigin.position, transform.forward * wallDistance);
    }
}
