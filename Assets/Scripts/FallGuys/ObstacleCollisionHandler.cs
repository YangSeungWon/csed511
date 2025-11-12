using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections;

/// <summary>
/// Handles obstacle collisions with knockback, haptic feedback, and audio
/// Reuses patterns from GunController.cs for Quest-compatible haptics
/// </summary>
public class ObstacleCollisionHandler : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.3f;

    [Header("Haptic Feedback Settings")]
    [SerializeField] private float hapticIntensity = 0.8f;
    [SerializeField] private float hapticDuration = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] collisionSounds;

    [Header("Controller References")]
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;

    private VRCharacterController vrCharacterController;
    private CharacterController characterController;
    private Coroutine knockbackCoroutine;

    void Awake()
    {
        vrCharacterController = GetComponent<VRCharacterController>();
        characterController = GetComponent<CharacterController>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if hit obstacle
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log($"Hit obstacle: {hit.gameObject.name}");
            HandleObstacleCollision(hit);
        }
    }

    /// <summary>
    /// Handle collision with obstacle
    /// </summary>
    private void HandleObstacleCollision(ControllerColliderHit hit)
    {
        // Apply knockback
        ApplyKnockback(hit.normal);

        // Trigger haptic feedback
        TriggerHapticFeedback();

        // Play collision audio
        PlayCollisionAudio();
    }

    /// <summary>
    /// Apply knockback force to player
    /// </summary>
    private void ApplyKnockback(Vector3 hitNormal)
    {
        Vector3 knockbackDirection = hitNormal.normalized;

        // Stop any existing knockback
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        // Start new knockback
        knockbackCoroutine = StartCoroutine(KnockbackCoroutine(knockbackDirection));
    }

    /// <summary>
    /// Coroutine to apply smooth knockback over time
    /// </summary>
    private IEnumerator KnockbackCoroutine(Vector3 direction)
    {
        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            // Gradually reduce knockback force over time
            float t = elapsed / knockbackDuration;
            float currentForce = Mathf.Lerp(knockbackForce, 0f, t);

            // Apply knockback movement
            if (vrCharacterController != null)
            {
                vrCharacterController.ApplyForce(direction * currentForce);
            }
            else if (characterController != null)
            {
                characterController.Move(direction * currentForce * Time.deltaTime);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        knockbackCoroutine = null;
    }

    /// <summary>
    /// Trigger haptic feedback on both controllers
    /// Uses dual method for Meta Quest compatibility (from GunController pattern)
    /// </summary>
    private void TriggerHapticFeedback()
    {
        // Method 1: ActionBasedController (XR Toolkit)
        if (leftController != null)
        {
            leftController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }

        if (rightController != null)
        {
            rightController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }

        // Method 2: InputDevice API (more reliable for Quest)
        TriggerHapticViaInputDevice(XRNode.LeftHand);
        TriggerHapticViaInputDevice(XRNode.RightHand);

        Debug.Log($"Haptic feedback triggered: intensity={hapticIntensity}, duration={hapticDuration}");
    }

    /// <summary>
    /// Trigger haptic via low-level InputDevice API (Meta Quest compatible)
    /// Pattern from GunController.cs
    /// </summary>
    private void TriggerHapticViaInputDevice(XRNode node)
    {
        // Get the input device for this hand
        UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(node);

        if (device.isValid)
        {
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0; // Oculus uses channel 0
                    device.SendHapticImpulse(channel, hapticIntensity, hapticDuration);
                    Debug.Log($"Haptic sent via InputDevice to {node}");
                }
            }
        }
    }

    /// <summary>
    /// Play collision audio
    /// </summary>
    private void PlayCollisionAudio()
    {
        if (audioSource != null && collisionSounds != null && collisionSounds.Length > 0)
        {
            AudioClip clip = collisionSounds[Random.Range(0, collisionSounds.Length)];
            audioSource.PlayOneShot(clip);
            Debug.Log($"Playing collision sound: {clip.name}");
        }
    }

    /// <summary>
    /// Set haptic intensity
    /// </summary>
    public void SetHapticIntensity(float intensity)
    {
        hapticIntensity = Mathf.Clamp01(intensity);
    }

    /// <summary>
    /// Set knockback force
    /// </summary>
    public void SetKnockbackForce(float force)
    {
        knockbackForce = force;
    }
}
