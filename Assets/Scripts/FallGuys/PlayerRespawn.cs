using UnityEngine;

/// <summary>
/// Respawns player at start position when falling off the map
/// </summary>
public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float respawnHeight = -10f;
    [SerializeField] private Vector3 respawnPosition = Vector3.zero;
    [SerializeField] private bool useStartPositionAsRespawn = true;

    private CharacterController characterController;
    private VRCharacterController vrCharacterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        vrCharacterController = GetComponent<VRCharacterController>();

        // Save start position as respawn point
        if (useStartPositionAsRespawn)
        {
            respawnPosition = transform.position;
        }
    }

    void Update()
    {
        // Check if player has fallen below respawn height
        if (transform.position.y < respawnHeight)
        {
            Respawn();
        }
    }

    /// <summary>
    /// Respawn player at designated position
    /// </summary>
    private void Respawn()
    {
        Debug.Log($"Player respawned at {respawnPosition}");

        // Disable CharacterController temporarily for teleport
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Reset position
        transform.position = respawnPosition;

        // Re-enable CharacterController
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // Reset VR character velocity if available
        if (vrCharacterController != null)
        {
            // VRCharacterController will reset velocity automatically on next frame
        }

        // Reset game state (includes timer and start/finish flags)
        if (FallGuysGameManager.Instance != null)
        {
            FallGuysGameManager.Instance.ResetGame();
            Debug.Log("Game reset on respawn");
        }
        else if (TimerManager.Instance != null)
        {
            // Fallback: reset timer directly if no game manager
            TimerManager.Instance.ResetTimer();
            Debug.Log("Timer reset on respawn");
        }
    }

    /// <summary>
    /// Set custom respawn position
    /// </summary>
    public void SetRespawnPosition(Vector3 position)
    {
        respawnPosition = position;
        useStartPositionAsRespawn = false;
    }

    /// <summary>
    /// Set respawn height threshold
    /// </summary>
    public void SetRespawnHeight(float height)
    {
        respawnHeight = height;
    }
}
