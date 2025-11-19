using UnityEngine;

/// <summary>
/// Breakable wall that breaks on high-speed collision
/// Some walls are rigid (unbreakable), others break above threshold speed
/// </summary>
public class BreakableWall : MonoBehaviour
{
    [Header("Break Settings")]
    [SerializeField] private bool isBreakable = true;
    [SerializeField] private float breakForceThreshold = 10f;
    [SerializeField] private bool requiresPlayerCollision = true;

    [Header("Break Effects")]
    [SerializeField] private GameObject brokenPiecesPrefab;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private float brokenPiecesLifetime = 3f;

    [Header("Rigid Wall (If Not Breakable)")]
    [SerializeField] private AudioClip impactSound;

    private AudioSource audioSource;
    private bool isBroken = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }

        // Tag this as obstacle for collision handler
        if (!gameObject.CompareTag("Obstacle"))
        {
            gameObject.tag = "Obstacle";
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken)
            return;

        // Check if collision is with player
        bool isPlayerCollision = collision.gameObject.CompareTag("Player") ||
                                 collision.gameObject.name.Contains("XR Origin");

        if (requiresPlayerCollision && !isPlayerCollision)
            return;

        // Calculate collision force
        float collisionForce = collision.relativeVelocity.magnitude;

        // For CharacterController, use alternative speed calculation
        CharacterController cc = collision.gameObject.GetComponent<CharacterController>();
        if (cc != null)
        {
            collisionForce = cc.velocity.magnitude;
        }

        Debug.Log($"Wall collision force: {collisionForce}");

        if (isBreakable && collisionForce >= breakForceThreshold)
        {
            Break();
        }
        else
        {
            // Play impact sound for rigid wall or low-force collision
            PlayImpactSound();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isBroken)
            return;

        // CharacterController collision (VR player)
        CharacterController cc = hit.controller;
        if (cc == null)
            return;

        // Calculate collision force from CharacterController velocity
        float collisionForce = cc.velocity.magnitude;
        Debug.Log($"Wall collision force (CharacterController): {collisionForce}");

        if (isBreakable && collisionForce >= breakForceThreshold)
        {
            Break();
        }
        else
        {
            // Play impact sound for rigid wall or low-force collision
            PlayImpactSound();
        }
    }

    /// <summary>
    /// Break the wall into pieces
    /// </summary>
    private void Break()
    {
        if (isBroken)
            return;

        isBroken = true;

        Debug.Log($"Wall broken: {gameObject.name}");

        // Play break sound
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        // Spawn broken pieces
        if (brokenPiecesPrefab != null)
        {
            GameObject pieces = Instantiate(brokenPiecesPrefab, transform.position, transform.rotation);
            Destroy(pieces, brokenPiecesLifetime);
        }
        else
        {
            // Create simple broken pieces from cubes
            CreateSimpleBrokenPieces();
        }

        // Destroy this wall
        Destroy(gameObject);
    }

    /// <summary>
    /// Create simple broken pieces if no prefab is assigned
    /// </summary>
    private void CreateSimpleBrokenPieces()
    {
        int pieceCount = 20; // Increased from 8 to 20
        float pieceSize = 0.2f; // Smaller pieces
        Vector3 size = GetComponent<Renderer>().bounds.size;

        for (int i = 0; i < pieceCount; i++)
        {
            // Create piece
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.transform.position = transform.position + Random.insideUnitSphere * (size.magnitude * 0.5f);
            piece.transform.localScale = Vector3.one * pieceSize;
            piece.transform.rotation = Random.rotation; // Random rotation for variety

            // Copy material
            Renderer renderer = piece.GetComponent<Renderer>();
            Renderer thisRenderer = GetComponent<Renderer>();
            if (thisRenderer != null && renderer != null)
            {
                renderer.material = thisRenderer.material;
            }

            // Add physics
            Rigidbody rb = piece.AddComponent<Rigidbody>();
            rb.mass = 0.5f; // Lighter pieces
            rb.AddExplosionForce(15f, transform.position, size.magnitude * 1.5f); // Very weak force, mostly just falls down

            // Auto-destroy
            Destroy(piece, brokenPiecesLifetime);
        }
    }

    /// <summary>
    /// Play impact sound for rigid walls
    /// </summary>
    private void PlayImpactSound()
    {
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound);
        }
    }

    /// <summary>
    /// Set whether wall is breakable
    /// </summary>
    public void SetBreakable(bool breakable)
    {
        isBreakable = breakable;
    }

    /// <summary>
    /// Set break force threshold
    /// </summary>
    public void SetBreakThreshold(float threshold)
    {
        breakForceThreshold = threshold;
    }

    /// <summary>
    /// Try to break the wall with given collision force
    /// Called externally from ObstacleCollisionHandler
    /// </summary>
    public void TryBreak(float collisionForce)
    {
        if (isBroken)
            return;

        Debug.Log($"Wall collision force: {collisionForce}, threshold: {breakForceThreshold}");

        if (isBreakable && collisionForce >= breakForceThreshold)
        {
            Break();
        }
        else
        {
            PlayImpactSound();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw breakable status
        Gizmos.color = isBreakable ? Color.yellow : Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
