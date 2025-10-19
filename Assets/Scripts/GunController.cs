using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[RequireComponent(typeof(XRGrabInteractable))]
public class GunController : MonoBehaviour
{
    [Header("Gun Properties")]
    [SerializeField] private Bullet.BulletColor bulletColor = Bullet.BulletColor.White;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint; // Point where bullets spawn
    [SerializeField] private float bulletSpeed = 20f;

    [Header("Firing Settings")]
    [SerializeField] private float fireRate = 0.2f; // Minimum time between shots
    private float lastFireTime = 0f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunShotSound;

    [Header("Haptic Feedback Settings")]
    [SerializeField] private float hapticIntensity = 0.5f;
    [SerializeField] private float hapticDuration = 0.1f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject muzzleFlashPrefab; // Optional muzzle flash effect
    [SerializeField] private float muzzleFlashDuration = 0.1f;

    private XRGrabInteractable grabInteractable;
    private ActionBasedController currentController; // Controller currently holding the gun
    private bool isTriggerPressed = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Configure audio source
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
    }

    void OnEnable()
    {
        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        // Unsubscribe from grab events
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    void Update()
    {
        // Check if gun is being held and trigger is pressed
        if (currentController != null)
        {
            CheckTriggerInput();
        }
    }

    /// <summary>
    /// Check for trigger input from the controller
    /// </summary>
    private void CheckTriggerInput()
    {
        // Read trigger value (0.0 to 1.0)
        float triggerValue = currentController.activateActionValue.action?.ReadValue<float>() ?? 0f;

        // Fire when trigger is pressed beyond threshold
        if (triggerValue > 0.7f && !isTriggerPressed)
        {
            isTriggerPressed = true;
            Fire();
        }
        else if (triggerValue < 0.3f)
        {
            isTriggerPressed = false; // Reset for next shot
        }
    }

    /// <summary>
    /// Fire the gun
    /// </summary>
    private void Fire()
    {
        // Check fire rate
        if (Time.time - lastFireTime < fireRate)
        {
            return;
        }

        lastFireTime = Time.time;

        // Spawn bullet
        SpawnBullet();

        // Play audio
        PlayGunShotAudio();

        // Trigger haptic feedback
        TriggerHapticFeedback();

        // Show muzzle flash
        ShowMuzzleFlash();

        Debug.Log($"{bulletColor} gun fired!");
    }

    /// <summary>
    /// Spawn a bullet at the muzzle point
    /// </summary>
    private void SpawnBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not assigned!");
            return;
        }

        // Determine spawn position and rotation
        Vector3 spawnPosition = muzzlePoint != null ? muzzlePoint.position : transform.position + transform.forward * 0.5f;
        Quaternion spawnRotation = muzzlePoint != null ? muzzlePoint.rotation : transform.rotation;

        // Instantiate bullet
        GameObject bulletObj = Instantiate(bulletPrefab, spawnPosition, spawnRotation);

        // Configure bullet
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetBulletColor(bulletColor);
        }

        // Set bullet color material
        SetBulletMaterial(bulletObj);
    }

    /// <summary>
    /// Set the visual color of the bullet based on bullet type
    /// </summary>
    private void SetBulletMaterial(GameObject bulletObj)
    {
        Renderer renderer = bulletObj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            switch (bulletColor)
            {
                case Bullet.BulletColor.Red:
                    mat.color = Color.red;
                    break;
                case Bullet.BulletColor.Blue:
                    mat.color = Color.blue;
                    break;
                case Bullet.BulletColor.White:
                    mat.color = Color.white;
                    break;
            }

            renderer.material = mat;
        }
    }

    /// <summary>
    /// Play gunshot sound
    /// </summary>
    private void PlayGunShotAudio()
    {
        if (audioSource != null && gunShotSound != null)
        {
            audioSource.PlayOneShot(gunShotSound);
        }
    }

    /// <summary>
    /// Trigger haptic feedback on the controller
    /// </summary>
    private void TriggerHapticFeedback()
    {
        if (currentController != null)
        {
            // Send haptic impulse to controller
            currentController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }

    /// <summary>
    /// Show muzzle flash effect
    /// </summary>
    private void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, muzzleFlashDuration);
        }
    }

    /// <summary>
    /// Called when gun is grabbed by a controller
    /// </summary>
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Get the controller that grabbed this gun
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            currentController = controllerInteractor.GetComponent<ActionBasedController>();
            Debug.Log($"{bulletColor} gun grabbed by {currentController.name}");
        }
    }

    /// <summary>
    /// Called when gun is released
    /// </summary>
    private void OnReleased(SelectExitEventArgs args)
    {
        currentController = null;
        isTriggerPressed = false;
        Debug.Log($"{bulletColor} gun released");
    }

    /// <summary>
    /// Set the bullet color for this gun
    /// </summary>
    public void SetBulletColor(Bullet.BulletColor color)
    {
        bulletColor = color;
    }

    /// <summary>
    /// Get the current bullet color
    /// </summary>
    public Bullet.BulletColor GetBulletColor()
    {
        return bulletColor;
    }
}
