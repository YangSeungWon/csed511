using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private float spawnInterval = 5f; // Spawn every 5 seconds

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints; // Predefined spawn locations
    [SerializeField] private bool randomizeSpawnPoints = true;

    [Header("Auto-Generation Settings")]
    [SerializeField] private bool autoGenerateSpawnPoints = false;
    [SerializeField] private int numSpawnPoints = 5;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float spawnHeight = 2f;

    [Header("Target Management")]
    [SerializeField] private int maxActiveTargets = 10; // Prevent too many targets
    private List<GameObject> activeTargets = new List<GameObject>();

    private float nextSpawnTime = 0f;
    private bool isSpawning = true;

    void Start()
    {
        // Auto-generate spawn points if needed
        if (autoGenerateSpawnPoints || spawnPoints == null || spawnPoints.Length == 0)
        {
            GenerateSpawnPoints();
        }

        // Set first spawn time
        nextSpawnTime = Time.time + spawnInterval;

        Debug.Log($"TargetSpawner initialized with {spawnPoints.Length} spawn points");
    }

    void Update()
    {
        // Check if it's time to spawn a new target
        if (isSpawning && Time.time >= nextSpawnTime)
        {
            SpawnTarget();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // Clean up destroyed targets from our tracking list
        CleanupDestroyedTargets();
    }

    /// <summary>
    /// Spawn a target at a random spawn point
    /// </summary>
    private void SpawnTarget()
    {
        // Check if we've reached max active targets
        if (activeTargets.Count >= maxActiveTargets)
        {
            Debug.LogWarning("Max active targets reached. Skipping spawn.");
            return;
        }

        if (targetPrefab == null)
        {
            Debug.LogError("Target prefab not assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points available!");
            return;
        }

        // Select spawn point
        Transform spawnPoint = GetRandomSpawnPoint();

        // Instantiate target
        GameObject target = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);

        // Track active target
        activeTargets.Add(target);

        Debug.Log($"Target spawned at {spawnPoint.position}. Active targets: {activeTargets.Count}");
    }

    /// <summary>
    /// Get a random spawn point from available points
    /// </summary>
    private Transform GetRandomSpawnPoint()
    {
        if (randomizeSpawnPoints)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex];
        }
        else
        {
            // Sequential spawning
            int index = (activeTargets.Count) % spawnPoints.Length;
            return spawnPoints[index];
        }
    }

    /// <summary>
    /// Generate spawn points in a circle around the spawner
    /// </summary>
    private void GenerateSpawnPoints()
    {
        List<Transform> generatedPoints = new List<Transform>();

        for (int i = 0; i < numSpawnPoints; i++)
        {
            // Calculate position in a circle
            float angle = i * (360f / numSpawnPoints);
            float radians = angle * Mathf.Deg2Rad;

            Vector3 position = transform.position + new Vector3(
                Mathf.Cos(radians) * spawnRadius,
                spawnHeight,
                Mathf.Sin(radians) * spawnRadius
            );

            // Create spawn point GameObject
            GameObject spawnPointObj = new GameObject($"SpawnPoint_{i}");
            spawnPointObj.transform.position = position;
            spawnPointObj.transform.parent = transform;

            // Point towards center
            spawnPointObj.transform.LookAt(transform.position + Vector3.up * spawnHeight);

            generatedPoints.Add(spawnPointObj.transform);
        }

        spawnPoints = generatedPoints.ToArray();
        Debug.Log($"Generated {numSpawnPoints} spawn points in a circle");
    }

    /// <summary>
    /// Remove null references from active targets list
    /// </summary>
    private void CleanupDestroyedTargets()
    {
        activeTargets.RemoveAll(target => target == null);
    }

    /// <summary>
    /// Start spawning targets
    /// </summary>
    public void StartSpawning()
    {
        isSpawning = true;
        nextSpawnTime = Time.time + spawnInterval;
        Debug.Log("Target spawning started");
    }

    /// <summary>
    /// Stop spawning targets
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("Target spawning stopped");
    }

    /// <summary>
    /// Clear all active targets
    /// </summary>
    public void ClearAllTargets()
    {
        foreach (GameObject target in activeTargets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }
        activeTargets.Clear();
        Debug.Log("All targets cleared");
    }

    /// <summary>
    /// Get count of active targets
    /// </summary>
    public int GetActiveTargetCount()
    {
        CleanupDestroyedTargets();
        return activeTargets.Count;
    }

    /// <summary>
    /// Draw spawn points in Scene view for debugging
    /// </summary>
    void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
                    Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.forward * 0.5f);
                }
            }
        }

        // Draw spawn radius if auto-generating
        if (autoGenerateSpawnPoints)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * spawnHeight, spawnRadius);
        }
    }
}
