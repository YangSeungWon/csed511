using UnityEngine;
using TMPro;
using System.IO;

/// <summary>
/// Manages game timer and persistent best time storage
/// Singleton pattern similar to ScoreManager from Assignment 3
/// </summary>
public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Timer Settings")]
    [SerializeField] private bool isRunning = false;
    private float currentTime = 0f;
    private float bestTime = float.MaxValue;
    private bool hasFinished = false;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI newRecordText;
    [SerializeField] private float newRecordDisplayTime = 3f;

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "fallguys_times.json";
    private string savePath;

    [System.Serializable]
    private class TimerData
    {
        public float bestTime;
    }

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Set save path
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        Debug.Log($"Timer save path: {savePath}");

        // Load best time
        LoadBestTime();

        // Hide new record text initially
        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    /// <summary>
    /// Start the timer
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
        currentTime = 0f;
        hasFinished = false;
        Debug.Log("Timer started!");
    }

    /// <summary>
    /// Stop the timer and check for new record
    /// </summary>
    public void StopTimer()
    {
        if (!isRunning || hasFinished)
            return;

        isRunning = false;
        hasFinished = true;

        Debug.Log($"Timer stopped! Final time: {FormatTime(currentTime)}");

        // Check if new best time
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            SaveBestTime();
            ShowNewRecordFeedback();
            Debug.Log($"New best time: {FormatTime(bestTime)}");
        }

        UpdateTimerDisplay();
    }

    /// <summary>
    /// Reset timer for new run
    /// </summary>
    public void ResetTimer()
    {
        isRunning = false;
        currentTime = 0f;
        hasFinished = false;
        UpdateTimerDisplay();

        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Update timer UI display
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (currentTimeText != null)
        {
            currentTimeText.text = $"Time: {FormatTime(currentTime)}";
        }

        if (bestTimeText != null)
        {
            if (bestTime < float.MaxValue)
            {
                bestTimeText.text = $"Best: {FormatTime(bestTime)}";
            }
            else
            {
                bestTimeText.text = "Best: --:--:--";
            }
        }
    }

    /// <summary>
    /// Format time as MM:SS.MS
    /// </summary>
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }

    /// <summary>
    /// Show "New Record!" feedback
    /// </summary>
    private void ShowNewRecordFeedback()
    {
        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(true);
            newRecordText.text = "NEW RECORD!";
            Invoke(nameof(HideNewRecordFeedback), newRecordDisplayTime);
        }
    }

    private void HideNewRecordFeedback()
    {
        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Save best time to persistent storage
    /// </summary>
    private void SaveBestTime()
    {
        try
        {
            TimerData data = new TimerData { bestTime = bestTime };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Best time saved: {FormatTime(bestTime)} to {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save best time: {e.Message}");
        }
    }

    /// <summary>
    /// Load best time from persistent storage
    /// </summary>
    private void LoadBestTime()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                TimerData data = JsonUtility.FromJson<TimerData>(json);
                bestTime = data.bestTime;
                Debug.Log($"Best time loaded: {FormatTime(bestTime)}");
                UpdateTimerDisplay();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load best time: {e.Message}");
                bestTime = float.MaxValue;
            }
        }
        else
        {
            Debug.Log("No saved best time found. Starting fresh!");
            bestTime = float.MaxValue;
        }
    }

    /// <summary>
    /// Get current time
    /// </summary>
    public float GetCurrentTime()
    {
        return currentTime;
    }

    /// <summary>
    /// Get best time
    /// </summary>
    public float GetBestTime()
    {
        return bestTime;
    }

    /// <summary>
    /// Check if timer is running
    /// </summary>
    public bool IsRunning()
    {
        return isRunning;
    }
}
