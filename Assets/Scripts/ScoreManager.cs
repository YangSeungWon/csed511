using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int currentScore = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hitFeedbackText; // Optional: shows recent hit feedback

    [Header("Feedback Settings")]
    [SerializeField] private float feedbackDisplayTime = 1f; // How long to show "+50" feedback

    private float feedbackTimer = 0f;
    private string currentFeedback = "";

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple ScoreManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        UpdateScoreDisplay();
    }

    void Update()
    {
        // Handle feedback text timeout
        if (feedbackTimer > 0)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0 && hitFeedbackText != null)
            {
                hitFeedbackText.text = "";
            }
        }
    }

    /// <summary>
    /// Add score based on bullet color
    /// </summary>
    public void AddScore(int points, Bullet.BulletColor bulletColor)
    {
        currentScore += points;
        UpdateScoreDisplay();

        // Show feedback
        ShowHitFeedback(points, bulletColor);

        Debug.Log($"Score added: +{points} ({bulletColor} bullet). Total score: {currentScore}");
    }

    /// <summary>
    /// Update the score UI display
    /// </summary>
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    /// <summary>
    /// Show temporary hit feedback (e.g., "+50 Red!")
    /// </summary>
    private void ShowHitFeedback(int points, Bullet.BulletColor bulletColor)
    {
        if (hitFeedbackText != null)
        {
            string colorName = bulletColor.ToString();
            hitFeedbackText.text = $"+{points} {colorName}!";
            feedbackTimer = feedbackDisplayTime;
        }
    }

    /// <summary>
    /// Reset score to zero
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
        Debug.Log("Score reset to 0");
    }

    /// <summary>
    /// Get current score value
    /// </summary>
    public int GetScore()
    {
        return currentScore;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
