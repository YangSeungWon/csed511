using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private float selfDestructTime = 5f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject destroyEffectPrefab; // Optional: particle effect on destroy

    private float spawnTime;
    private bool isDestroyed = false;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        // Self-destruct after 5 seconds
        if (!isDestroyed && Time.time - spawnTime >= selfDestructTime)
        {
            DestroyTarget(false); // false = self-destruct, not hit by bullet
        }
    }

    /// <summary>
    /// Destroys the target with optional scoring
    /// </summary>
    /// <param name="hitByBullet">True if destroyed by bullet hit, false if self-destructed</param>
    public void DestroyTarget(bool hitByBullet)
    {
        if (isDestroyed) return; // Prevent double destruction
        isDestroyed = true;

        // Spawn destroy effect if available
        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }

        // Visual/audio feedback could be added here
        if (hitByBullet)
        {
            Debug.Log($"Target destroyed by bullet at {transform.position}");
        }
        else
        {
            Debug.Log($"Target self-destructed at {transform.position}");
        }

        // Destroy the game object
        Destroy(gameObject);
    }

    /// <summary>
    /// Get remaining lifetime of the target
    /// </summary>
    public float GetRemainingLifetime()
    {
        return Mathf.Max(0, selfDestructTime - (Time.time - spawnTime));
    }

    /// <summary>
    /// Check if target is still alive
    /// </summary>
    public bool IsAlive()
    {
        return !isDestroyed;
    }

    void OnDestroy()
    {
        // Cleanup if needed
    }
}
