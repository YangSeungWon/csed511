using UnityEngine;

/// <summary>
/// Main game manager for Fall Guys VR game
/// Manages game state, start/finish triggers, and overall flow
/// </summary>
public class FallGuysGameManager : MonoBehaviour
{
    public static FallGuysGameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private bool gameFinished = false;

    [Header("Start/Finish Zones")]
    [SerializeField] private Transform startZone;
    [SerializeField] private Transform finishZone;

    [Header("Player Reference")]
    [SerializeField] private GameObject player; // XR Origin

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
    }

    void Start()
    {
        Debug.Log("Fall Guys Game Manager initialized!");
    }

    /// <summary>
    /// Called when player enters start zone
    /// </summary>
    public void OnStartZoneEntered()
    {
        if (gameStarted || gameFinished)
            return;

        gameStarted = true;
        Debug.Log("Game started!");

        // Start timer
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StartTimer();
        }
    }

    /// <summary>
    /// Called when player reaches finish zone
    /// </summary>
    public void OnFinishZoneEntered()
    {
        if (!gameStarted || gameFinished)
            return;

        gameFinished = true;
        Debug.Log("Game finished!");

        // Stop timer
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }
    }

    /// <summary>
    /// Reset game for new run
    /// </summary>
    public void ResetGame()
    {
        gameStarted = false;
        gameFinished = false;

        // Reset timer
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.ResetTimer();
        }

        // Teleport player back to start
        if (player != null && startZone != null)
        {
            player.transform.position = startZone.position;
            player.transform.rotation = startZone.rotation;
        }

        Debug.Log("Game reset!");
    }

    /// <summary>
    /// Check if game is active
    /// </summary>
    public bool IsGameActive()
    {
        return gameStarted && !gameFinished;
    }

    /// <summary>
    /// Check if game is finished
    /// </summary>
    public bool IsGameFinished()
    {
        return gameFinished;
    }
}
