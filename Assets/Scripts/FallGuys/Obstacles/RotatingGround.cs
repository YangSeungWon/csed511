using UnityEngine;

/// <summary>
/// Rotating or moving ground platform
/// Players must maintain balance or fall off
/// </summary>
public class RotatingGround : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Movement Settings")]
    [SerializeField] private bool enableMovement = false;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 5f;

    [Header("Physics Settings")]
    [SerializeField] private PhysicMaterial slipperyMaterial;

    private Vector3 startPosition;
    private float moveProgress = 0f;
    private int moveDir = 1; // 1 or -1 for ping-pong movement

    void Start()
    {
        startPosition = transform.position;

        // NOTE: This is a platform, NOT an obstacle
        // Do NOT tag as "Obstacle" - platforms should not trigger knockback/haptics

        // Apply slippery material if available
        if (slipperyMaterial != null)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.material = slipperyMaterial;
            }
        }
    }

    void Update()
    {
        // Handle rotation
        if (enableRotation)
        {
            transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.Self);
        }

        // Handle movement
        if (enableMovement)
        {
            UpdateMovement();
        }
    }

    /// <summary>
    /// Update platform movement (ping-pong)
    /// </summary>
    private void UpdateMovement()
    {
        moveProgress += moveSpeed * moveDir * Time.deltaTime;

        // Reverse direction at limits
        if (moveProgress >= moveDistance)
        {
            moveProgress = moveDistance;
            moveDir = -1;
        }
        else if (moveProgress <= 0f)
        {
            moveProgress = 0f;
            moveDir = 1;
        }

        // Apply movement
        transform.position = startPosition + moveDirection.normalized * moveProgress;
    }

    /// <summary>
    /// Set rotation speed
    /// </summary>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    /// <summary>
    /// Set rotation axis
    /// </summary>
    public void SetRotationAxis(Vector3 axis)
    {
        rotationAxis = axis.normalized;
    }

    /// <summary>
    /// Set movement speed
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    /// <summary>
    /// Enable/disable rotation
    /// </summary>
    public void SetRotationEnabled(bool enabled)
    {
        enableRotation = enabled;
    }

    /// <summary>
    /// Enable/disable movement
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        enableMovement = enabled;
    }

    void OnDrawGizmos()
    {
        // Draw rotation axis
        if (enableRotation)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + rotationAxis.normalized * 2f);
        }

        // Draw movement path
        if (enableMovement)
        {
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Vector3 end = start + moveDirection.normalized * moveDistance;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(start, 0.2f);
            Gizmos.DrawWireSphere(end, 0.2f);
        }
    }
}
