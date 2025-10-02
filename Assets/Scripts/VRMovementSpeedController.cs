using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRMovementSpeedController : MonoBehaviour
{
    [Header("Movement Speed Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float dashSpeed = 6f;

    [Header("References")]
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;

    private int currentSpeedIndex = 0; // 0: walk, 1: run, 2: dash
    private float[] speeds;

    public string CurrentSpeedName
    {
        get
        {
            switch (currentSpeedIndex)
            {
                case 0: return "Walk";
                case 1: return "Run";
                case 2: return "Dash";
                default: return "Unknown";
            }
        }
    }

    public float CurrentSpeed => speeds[currentSpeedIndex];

    void Start()
    {
        // Initialize speed array
        speeds = new float[] { walkSpeed, runSpeed, dashSpeed };

        // Find move provider if not assigned
        if (moveProvider == null)
        {
            moveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
        }

        // Set initial speed
        UpdateMoveSpeed();
    }

    /// <summary>
    /// Cycle through speed levels: Walk → Run → Dash → Walk
    /// Called by UI button
    /// </summary>
    public void CycleSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speeds.Length;
        UpdateMoveSpeed();
        Debug.Log($"Movement speed changed to: {CurrentSpeedName} ({CurrentSpeed} m/s)");
    }

    /// <summary>
    /// Set specific speed by index (0: walk, 1: run, 2: dash)
    /// </summary>
    public void SetSpeedIndex(int index)
    {
        if (index >= 0 && index < speeds.Length)
        {
            currentSpeedIndex = index;
            UpdateMoveSpeed();
        }
    }

    private void UpdateMoveSpeed()
    {
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = speeds[currentSpeedIndex];
        }
    }
}
