using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class VRTriggerStateMonitor : MonoBehaviour
{
    [Header("Controller References")]
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;

    public enum TriggerState
    {
        NotPressed,
        HalfPressed,
        FullyPressed
    }

    public TriggerState LeftTriggerState { get; private set; } = TriggerState.NotPressed;
    public TriggerState RightTriggerState { get; private set; } = TriggerState.NotPressed;

    [Header("Trigger Thresholds")]
    [SerializeField] private float halfPressThreshold = 0.3f;
    [SerializeField] private float fullPressThreshold = 0.9f;

    void Update()
    {
        UpdateTriggerState();
    }

    private void UpdateTriggerState()
    {
        if (leftController != null)
        {
            float leftValue = leftController.activateActionValue.action?.ReadValue<float>() ?? 0f;
            LeftTriggerState = GetTriggerState(leftValue);
        }

        if (rightController != null)
        {
            float rightValue = rightController.activateActionValue.action?.ReadValue<float>() ?? 0f;
            RightTriggerState = GetTriggerState(rightValue);
        }
    }

    private TriggerState GetTriggerState(float value)
    {
        if (value >= fullPressThreshold)
            return TriggerState.FullyPressed;
        else if (value >= halfPressThreshold)
            return TriggerState.HalfPressed;
        else
            return TriggerState.NotPressed;
    }

    public string GetTriggerStateString(TriggerState state)
    {
        switch (state)
        {
            case TriggerState.NotPressed:
                return "Not pressed";
            case TriggerState.HalfPressed:
                return "Half pressed";
            case TriggerState.FullyPressed:
                return "Fully pressed";
            default:
                return "Unknown";
        }
    }
}
