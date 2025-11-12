using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Manages drone flyover camera animation at game start
/// Transitions from cinematic view to player VR view
/// </summary>
public class DroneCamera : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera droneCamera;
    [SerializeField] private Camera vrCamera;
    [SerializeField] private GameObject xrOrigin;

    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private float flyoverDuration = 4f;

    [Header("Movement Components to Disable")]
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private ActionBasedSnapTurnProvider turnProvider;
    [SerializeField] private VRCharacterController vrCharacterController;

    [Header("UI")]
    [SerializeField] private GameObject titleUI; // "Only Up - [Name]"

    private bool flyoverComplete = false;

    void Start()
    {
        StartFlyover();
    }

    /// <summary>
    /// Start the flyover sequence
    /// </summary>
    public void StartFlyover()
    {
        Debug.Log("Starting drone flyover...");

        // Disable player control
        DisablePlayerControl();

        // Enable drone camera
        if (droneCamera != null)
        {
            droneCamera.enabled = true;
        }

        // Disable VR camera
        if (vrCamera != null)
        {
            vrCamera.enabled = false;
        }

        // Show title UI
        if (titleUI != null)
        {
            titleUI.SetActive(true);
        }

        // Play timeline if available
        if (timeline != null)
        {
            timeline.Play();
            timeline.stopped += OnFlyoverComplete;
        }
        else
        {
            // If no timeline, use simple timed transition
            Invoke(nameof(EndFlyover), flyoverDuration);
        }
    }

    /// <summary>
    /// End the flyover and transition to VR view
    /// </summary>
    public void EndFlyover()
    {
        if (flyoverComplete)
            return;

        flyoverComplete = true;

        Debug.Log("Flyover complete! Switching to VR view...");

        // Hide title UI
        if (titleUI != null)
        {
            titleUI.SetActive(false);
        }

        // Disable drone camera
        if (droneCamera != null)
        {
            droneCamera.enabled = false;
        }

        // Enable VR camera
        if (vrCamera != null)
        {
            vrCamera.enabled = true;
        }

        // Enable player control
        EnablePlayerControl();

        // Start game timer
        if (TimerManager.Instance != null)
        {
            // Timer will start when player enters start zone
            Debug.Log("Ready to start!");
        }
    }

    /// <summary>
    /// Disable player movement and rotation
    /// </summary>
    private void DisablePlayerControl()
    {
        if (moveProvider != null)
        {
            moveProvider.enabled = false;
        }

        if (turnProvider != null)
        {
            turnProvider.enabled = false;
        }

        if (vrCharacterController != null)
        {
            vrCharacterController.enabled = false;
        }
    }

    /// <summary>
    /// Enable player movement and rotation
    /// </summary>
    private void EnablePlayerControl()
    {
        if (moveProvider != null)
        {
            moveProvider.enabled = true;
        }

        if (turnProvider != null)
        {
            turnProvider.enabled = true;
        }

        if (vrCharacterController != null)
        {
            vrCharacterController.enabled = true;
        }
    }

    /// <summary>
    /// Called when timeline finishes
    /// </summary>
    private void OnFlyoverComplete(PlayableDirector director)
    {
        EndFlyover();
    }

    /// <summary>
    /// Skip flyover (for testing)
    /// </summary>
    public void SkipFlyover()
    {
        if (timeline != null && timeline.state == PlayState.Playing)
        {
            timeline.Stop();
        }

        CancelInvoke(nameof(EndFlyover));
        EndFlyover();
    }
}
