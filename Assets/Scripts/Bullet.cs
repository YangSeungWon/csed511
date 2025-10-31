using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private int scoreValue = 10; // Points awarded when this bullet hits a target
    [SerializeField] private float invincibilityTime = 0.1f; // Time before bullet can collide with objects

    [Header("Bullet Type")]
    public BulletColor bulletColor = BulletColor.White;

    private Rigidbody rb;
    private Collider bulletCollider;
    private float spawnTime;
    private bool isInvincible = true;

    public enum BulletColor
    {
        Red,    // +50 points
        Blue,   // +30 points
        White   // +10 points
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configure Rigidbody
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Get collider and disable it initially for invincibility period
        bulletCollider = GetComponent<Collider>();
        if (bulletCollider != null)
        {
            bulletCollider.enabled = false;
        }
    }

    void Start()
    {
        spawnTime = Time.time;

        // Set score value based on bullet color
        switch (bulletColor)
        {
            case BulletColor.Red:
                scoreValue = 50;
                break;
            case BulletColor.Blue:
                scoreValue = 30;
                break;
            case BulletColor.White:
                scoreValue = 10;
                break;
        }

        // Apply forward velocity
        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        // Enable collider after invincibility time
        if (isInvincible && Time.time - spawnTime >= invincibilityTime)
        {
            isInvincible = false;
            if (bulletCollider != null)
            {
                bulletCollider.enabled = true;
            }
        }

        // Self-destruct after lifetime expires
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit a target
        Target target = collision.gameObject.GetComponent<Target>();
        if (target != null)
        {
            // Notify score manager
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue, bulletColor);
            }

            // Destroy the target
            target.DestroyTarget(true); // true = destroyed by bullet hit

            // Destroy this bullet
            Destroy(gameObject);
        }
        else
        {
            // Hit something else (wall, floor, etc.)
            // Optionally destroy bullet on any collision to prevent clutter
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the bullet's color and configure score value accordingly
    /// </summary>
    public void SetBulletColor(BulletColor color)
    {
        bulletColor = color;

        // Set score value
        switch (bulletColor)
        {
            case BulletColor.Red:
                scoreValue = 50;
                break;
            case BulletColor.Blue:
                scoreValue = 30;
                break;
            case BulletColor.White:
                scoreValue = 10;
                break;
        }
    }

    public int GetScoreValue()
    {
        return scoreValue;
    }
}
