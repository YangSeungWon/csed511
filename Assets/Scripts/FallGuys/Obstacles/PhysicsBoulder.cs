using UnityEngine;

/// <summary>
/// Simple physics-based rolling boulder
/// Rolls down slope with gravity, despawns when falling off cliff
/// </summary>
public class PhysicsBoulder : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float despawnHeight = -10f;
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private bool autoRespawn = true;

    [Header("Physics Settings")]
    [SerializeField] private float mass = 100f;
    [SerializeField] private float initialSpeed = 5f; // Initial forward push

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isActive = true;

    void Awake()
    {
        // Save spawn position
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Setup Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.mass = mass;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Tag as obstacle
        if (!gameObject.CompareTag("Obstacle"))
        {
            gameObject.tag = "Obstacle";
        }
    }

    void Update()
    {
        if (!isActive)
            return;

        // Check if fallen off cliff
        if (transform.position.y < despawnHeight)
        {
            Despawn();
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
    /// Respawn boulder at start position
    /// </summary>
    private void Respawn()
    {
        // Reset position and rotation
        transform.position = startPosition;
        transform.rotation = startRotation;

        // Reset physics
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Give initial forward push
            if (initialSpeed > 0)
            {
                rb.AddForce(transform.forward * initialSpeed, ForceMode.VelocityChange);
            }
        }

        // Reactivate
        gameObject.SetActive(true);
        isActive = true;

        Debug.Log($"Boulder respawned at {startPosition}");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if hit player
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.name.Contains("XR Origin"))
        {
            Debug.Log($"Boulder hit player: {collision.gameObject.name}");
        }
    }
}
