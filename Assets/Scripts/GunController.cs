using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR;

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

    // Store disabled interactors to re-enable them when gun is released
    private XRRayInteractor rayInteractor;
    private XRDirectInteractor directInteractor;
    private XRBaseInteractor grabbingInteractor; // The interactor currently holding this gun

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
            // Method 1: Try ActionBasedController (XR Interaction Toolkit)
            currentController.SendHapticImpulse(hapticIntensity, hapticDuration);
            Debug.Log($"Haptic feedback sent via ActionBasedController: intensity={hapticIntensity}, duration={hapticDuration}");

            // Method 2: Also try direct InputDevice API (more reliable for Quest)
            TriggerHapticViaInputDevice();
        }
        else
        {
            Debug.LogWarning("Cannot send haptic feedback: currentController is null!");
        }
    }

    /// <summary>
    /// Trigger haptic via low-level InputDevice API (more reliable for Meta Quest)
    /// </summary>
    private void TriggerHapticViaInputDevice()
    {
        // Determine which hand is holding the gun
        XRNode node = XRNode.RightHand; // Default to right
        if (currentController != null && currentController.name.ToLower().Contains("left"))
        {
            node = XRNode.LeftHand;
        }

        // Get the input device for this hand (explicitly use UnityEngine.XR.InputDevice)
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
                    Debug.Log($"Haptic sent via InputDevice to {node}: intensity={hapticIntensity}, duration={hapticDuration}");
                }
                else
                {
                    Debug.LogWarning($"Device {device.name} does not support haptic impulse!");
                }
            }
        }
        else
        {
            Debug.LogWarning($"InputDevice for {node} is not valid!");
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
            grabbingInteractor = controllerInteractor;
            Debug.Log($"{bulletColor} gun grabbed by {currentController.name}");

            // Disable other interactors on this controller to prevent teleporting or grabbing other objects
            DisableOtherInteractors(controllerInteractor.transform);
        }
    }

    /// <summary>
    /// Called when gun is released
    /// </summary>
    private void OnReleased(SelectExitEventArgs args)
    {
        // Re-enable other interactors
        EnableOtherInteractors();

        currentController = null;
        grabbingInteractor = null;
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

    /// <summary>
    /// Disable other interactors on the controller to prevent teleporting or grabbing while holding gun
    /// </summary>
    private void DisableOtherInteractors(Transform controllerTransform)
    {
        // Find and disable Ray Interactor (used for teleportation and UI)
        // But DON'T disable the interactor currently holding the gun
        rayInteractor = controllerTransform.GetComponentInChildren<XRRayInteractor>();
        if (rayInteractor != null && rayInteractor != grabbingInteractor)
        {
            rayInteractor.enabled = false;
            Debug.Log($"Disabled Ray Interactor on {controllerTransform.name}");
        }
        else if (rayInteractor == grabbingInteractor)
        {
            rayInteractor = null; // Don't track it since we're not disabling it
        }

        // Find and disable Direct Interactor (used for grabbing nearby objects)
        // But DON'T disable the interactor currently holding the gun
        directInteractor = controllerTransform.GetComponentInChildren<XRDirectInteractor>();
        if (directInteractor != null && directInteractor != grabbingInteractor)
        {
            directInteractor.enabled = false;
            Debug.Log($"Disabled Direct Interactor on {controllerTransform.name}");
        }
        else if (directInteractor == grabbingInteractor)
        {
            directInteractor = null; // Don't track it since we're not disabling it
        }
    }

    /// <summary>
    /// Re-enable other interactors when gun is released
    /// </summary>
    private void EnableOtherInteractors()
    {
        // Re-enable Ray Interactor
        if (rayInteractor != null)
        {
            rayInteractor.enabled = true;
            Debug.Log("Re-enabled Ray Interactor");
            rayInteractor = null;
        }

        // Re-enable Direct Interactor
        if (directInteractor != null)
        {
            directInteractor.enabled = true;
            Debug.Log("Re-enabled Direct Interactor");
            directInteractor = null;
        }
    }
}
