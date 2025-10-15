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

    // Index trigger (Activate Action)
    public TriggerState LeftIndexTriggerState { get; private set; } = TriggerState.NotPressed;
    public TriggerState RightIndexTriggerState { get; private set; } = TriggerState.NotPressed;

    // Grip button (Select Action)
    public TriggerState LeftGripState { get; private set; } = TriggerState.NotPressed;
    public TriggerState RightGripState { get; private set; } = TriggerState.NotPressed;

    // Backwards compatibility
    public TriggerState LeftTriggerState => LeftIndexTriggerState;
    public TriggerState RightTriggerState => RightIndexTriggerState;

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
            // Index trigger (Activate)
            float leftActivateValue = leftController.activateActionValue.action?.ReadValue<float>() ?? 0f;
            LeftIndexTriggerState = GetTriggerState(leftActivateValue);

            // Grip button (Select)
            float leftSelectValue = leftController.selectActionValue.action?.ReadValue<float>() ?? 0f;
            LeftGripState = GetTriggerState(leftSelectValue);
        }

        if (rightController != null)
        {
            // Index trigger (Activate)
            float rightActivateValue = rightController.activateActionValue.action?.ReadValue<float>() ?? 0f;
            RightIndexTriggerState = GetTriggerState(rightActivateValue);

            // Grip button (Select)
            float rightSelectValue = rightController.selectActionValue.action?.ReadValue<float>() ?? 0f;
            RightGripState = GetTriggerState(rightSelectValue);
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

    public string GetLeftControllerStateString()
    {
        return $"Trigger: {GetTriggerStateString(LeftIndexTriggerState)}\n" +
               $"Grip: {GetTriggerStateString(LeftGripState)}";
    }

    public string GetRightControllerStateString()
    {
        return $"Trigger: {GetTriggerStateString(RightIndexTriggerState)}\n" +
               $"Grip: {GetTriggerStateString(RightGripState)}";
    }
}
