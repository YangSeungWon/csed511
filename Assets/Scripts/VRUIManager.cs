using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VRUIManager : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private VRTriggerStateMonitor triggerMonitor;
    [SerializeField] private VRObjectInfoMonitor objectInfoMonitor;
    [SerializeField] private VRMovementSpeedController speedController;
    [SerializeField] private VRObjectSpawner objectSpawner;

    [Header("UI Elements - Trigger Status")]
    [SerializeField] private TextMeshProUGUI leftTriggerText;
    [SerializeField] private TextMeshProUGUI rightTriggerText;

    [Header("UI Elements - Object Info")]
    [SerializeField] private TextMeshProUGUI objectInfoText;

    [Header("UI Elements - Speed Control")]
    [SerializeField] private TextMeshProUGUI speedDisplayText;
    [SerializeField] private Button speedButton;

    [Header("UI Elements - Spawner Buttons")]
    [SerializeField] private Transform spawnerButtonContainer;
    [SerializeField] private GameObject spawnerButtonPrefab;

    [Header("UI Elements - Clear Button")]
    [SerializeField] private Button clearButton;

    void Start()
    {
        // Setup speed button
        if (speedButton != null)
        {
            speedButton.onClick.AddListener(OnSpeedButtonClicked);
        }

        // Setup clear button
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(OnClearButtonClicked);
        }

        // Create spawner buttons dynamically
        CreateSpawnerButtons();
    }

    void Update()
    {
        UpdateTriggerStatus();
        UpdateObjectInfo();
        UpdateSpeedDisplay();
    }

    private void UpdateTriggerStatus()
    {
        if (triggerMonitor == null) return;

        if (leftTriggerText != null)
        {
            string leftState = triggerMonitor.GetTriggerStateString(triggerMonitor.LeftTriggerState);
            leftTriggerText.text = $"Left: {leftState}";
        }

        if (rightTriggerText != null)
        {
            string rightState = triggerMonitor.GetTriggerStateString(triggerMonitor.RightTriggerState);
            rightTriggerText.text = $"Right: {rightState}";
        }
    }

    private void UpdateObjectInfo()
    {
        if (objectInfoMonitor == null || objectInfoText == null) return;

        objectInfoText.text = objectInfoMonitor.GetObjectInfoString();
    }

    private void UpdateSpeedDisplay()
    {
        if (speedController == null || speedDisplayText == null) return;

        speedDisplayText.text = $"Speed: {speedController.CurrentSpeedName}\n({speedController.CurrentSpeed:F1} m/s)";
    }

    private void OnSpeedButtonClicked()
    {
        if (speedController != null)
        {
            speedController.CycleSpeed();
        }
    }

    private void OnClearButtonClicked()
    {
        if (objectSpawner != null)
        {
            objectSpawner.ClearAllSpawnedObjects();
        }
    }

    private void CreateSpawnerButtons()
    {
        if (objectSpawner == null || spawnerButtonContainer == null)
        {
            Debug.LogWarning("Object spawner or spawner button container not assigned!");
            return;
        }

        int count = objectSpawner.GetSpawnableObjectCount();
        for (int i = 0; i < count; i++)
        {
            // If prefab is provided, instantiate it; otherwise create simple button
            Button btn;
            if (spawnerButtonPrefab != null)
            {
                GameObject btnObj = Instantiate(spawnerButtonPrefab, spawnerButtonContainer);
                btn = btnObj.GetComponent<Button>();
            }
            else
            {
                GameObject btnObj = new GameObject($"Spawn Button {i}");
                btnObj.transform.SetParent(spawnerButtonContainer, false);
                btn = btnObj.AddComponent<Button>();

                // Add text child
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(btnObj.transform, false);
                TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
                btnText.alignment = TextAlignmentOptions.Center;
                btnText.fontSize = 24;

                // Add Image component for button visual
                Image img = btnObj.AddComponent<Image>();
                img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

                // Set text
                btnText.text = objectSpawner.GetObjectName(i);
            }

            // Setup button text
            TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = objectSpawner.GetObjectName(i);
            }

            // Add click listener
            int index = i; // Capture index for closure
            btn.onClick.AddListener(() => OnSpawnerButtonClicked(index));
        }
    }

    private void OnSpawnerButtonClicked(int index)
    {
        if (objectSpawner != null)
        {
            objectSpawner.SpawnObject(index);
        }
    }
}
