using UnityEngine;

public class RollerTrap : MonoBehaviour
{
    public enum Directions { Forward, Backward, Left, Right }
    public Directions Direction;

    public float pushSpeed = 4f;
    public bool onlyWhenGrounded = true;

    private CharacterController target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            target = other.GetComponent<CharacterController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (target != null && other.gameObject == target.gameObject)
        {
            target = null;
        }
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }
        if (onlyWhenGrounded && !target.isGrounded)
        {
            return;
        }

        Vector3 push = GetDirection() * pushSpeed;
        push.y = -2f;

        target.Move(push * Time.deltaTime);
    }

    private Vector3 GetDirection()
    {
        switch (Direction)
        {
            case Directions.Forward: return Vector3.right;    // X+
            case Directions.Backward: return Vector3.left;     // X-
            case Directions.Left: return Vector3.forward;  // Z+
            case Directions.Right: return Vector3.back;     // Z-
        }
        return Vector3.zero;
    }
}