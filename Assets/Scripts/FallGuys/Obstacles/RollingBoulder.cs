using UnityEngine;

/// <summary>
/// Rolling boulder that moves along a path
/// Despawns when falling off cliff or reaching end
/// </summary>
public class RollingBoulder : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool loop = true;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Spawn Settings")]
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private bool autoRespawn = true;
    [SerializeField] private float despawnHeight = -10f; // Y position to despawn

    [Header("Physics Settings")]
    [SerializeField] private bool usePhysics = false; // Use Rigidbody physics or transform movement

    private int currentWaypoint = 0;
    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isActive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Tag as obstacle
        if (!gameObject.CompareTag("Obstacle"))
        {
            gameObject.tag = "Obstacle";
        }

        // Setup physics
        if (usePhysics && rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 10f;
            rb.useGravity = true;
        }
        else if (!usePhysics && rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!isActive)
            return;

        // Check if fallen off
        if (transform.position.y < despawnHeight)
        {
            Despawn();
            return;
        }

        // Move boulder
        if (!usePhysics)
        {
            MoveAlongWaypoints();
        }

        // Rotate for rolling effect
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
    }

    void FixedUpdate()
    {
        if (!isActive || !usePhysics || rb == null)
            return;

        // Physics-based movement
        if (waypoints != null && waypoints.Length > 0)
        {
            Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
            rb.AddForce(direction * moveSpeed, ForceMode.Force);
        }
    }

    /// <summary>
    /// Move boulder along waypoint path
    /// </summary>
    private void MoveAlongWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            // No waypoints, just move forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
            return;
        }

        // Move towards current waypoint
        Vector3 targetPos = waypoints[currentWaypoint].position;
        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // Check if reached waypoint
        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance < 0.5f)
        {
            currentWaypoint++;

            // Check if reached end
            if (currentWaypoint >= waypoints.Length)
            {
                if (loop)
                {
                    currentWaypoint = 0;
                }
                else
                {
                    Despawn();
                }
            }
        }
    }

    /// <summary>
    /// Despawn boulder
    /// </summary>
    private void Despawn()
    {
        isActive = false;
        gameObject.SetActive(false);

        if (autoRespawn)
        {
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    /// <summary>
    /// Respawn boulder at start
    /// </summary>
    private void Respawn()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        currentWaypoint = 0;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(true);
        isActive = true;

        Debug.Log($"Boulder respawned: {gameObject.name}");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if hit player
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.name.Contains("XR Origin"))
        {
            Debug.Log($"Boulder hit player: {collision.gameObject.name}");
            // Collision handler will trigger knockback/haptic/audio
        }
    }

    /// <summary>
    /// Set move speed
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    /// <summary>
    /// Set rotation speed
    /// </summary>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    /// <summary>
    /// Trigger manual despawn
    /// </summary>
    public void ForceDespawn()
    {
        Despawn();
    }

    void OnDrawGizmos()
    {
        // Draw waypoint path
        if (waypoints != null && waypoints.Length > 1)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null)
                    continue;

                // Draw waypoint sphere
                Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);

                // Draw line to next waypoint
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
                else if (loop && waypoints[0] != null)
                {
                    // Draw line back to start if looping
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
        }

        // Draw despawn height line
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position;
        Gizmos.DrawLine(new Vector3(pos.x - 5, despawnHeight, pos.z),
                        new Vector3(pos.x + 5, despawnHeight, pos.z));
    }
}
