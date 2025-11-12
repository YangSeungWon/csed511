using UnityEngine;

/// <summary>
/// Trigger zone for start and finish lines
/// Detects when player enters to start/stop timer
/// </summary>
public class StartFinishTrigger : MonoBehaviour
{
    [Header("Trigger Type")]
    [SerializeField] private TriggerType triggerType = TriggerType.Start;

    [Header("Visual Feedback")]
    [SerializeField] private Color gizmoColor = Color.green;

    public enum TriggerType
    {
        Start,
        Finish
    }

    void Awake()
    {
        // Ensure this has a trigger collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if player entered
        if (IsPlayer(other))
        {
            HandleTrigger();
        }
    }

    /// <summary>
    /// Check if collider is the player
    /// </summary>
    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") ||
               other.name.Contains("XR Origin") ||
               other.GetComponentInParent<VRCharacterController>() != null;
    }

    /// <summary>
    /// Handle trigger activation
    /// </summary>
    private void HandleTrigger()
    {
        switch (triggerType)
        {
            case TriggerType.Start:
                HandleStart();
                break;

            case TriggerType.Finish:
                HandleFinish();
                break;
        }
    }

    /// <summary>
    /// Handle start trigger
    /// </summary>
    private void HandleStart()
    {
        Debug.Log("Player entered START zone");

        if (FallGuysGameManager.Instance != null)
        {
            FallGuysGameManager.Instance.OnStartZoneEntered();
        }
        else if (TimerManager.Instance != null)
        {
            // Fallback: start timer directly
            TimerManager.Instance.StartTimer();
        }
    }

    /// <summary>
    /// Handle finish trigger
    /// </summary>
    private void HandleFinish()
    {
        Debug.Log("Player entered FINISH zone");

        if (FallGuysGameManager.Instance != null)
        {
            FallGuysGameManager.Instance.OnFinishZoneEntered();
        }
        else if (TimerManager.Instance != null)
        {
            // Fallback: stop timer directly
            TimerManager.Instance.StopTimer();
        }
    }

    /// <summary>
    /// Set trigger type
    /// </summary>
    public void SetTriggerType(TriggerType type)
    {
        triggerType = type;

        // Update gizmo color
        switch (type)
        {
            case TriggerType.Start:
                gizmoColor = Color.green;
                break;
            case TriggerType.Finish:
                gizmoColor = Color.red;
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            if (collider is BoxCollider boxCollider)
            {
                Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                Gizmos.DrawWireSphere(transform.position + sphereCollider.center, sphereCollider.radius);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, 1f);
            }
        }
    }
}
