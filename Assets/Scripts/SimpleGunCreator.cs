using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Editor script to quickly create simple gun prototypes from primitives
/// Attach to an empty GameObject and it will create a basic gun shape
/// </summary>
public class SimpleGunCreator : MonoBehaviour
{
    [Header("Gun Configuration")]
    [SerializeField] private Bullet.BulletColor gunColor = Bullet.BulletColor.White;
    [SerializeField] private Color visualColor = Color.white;

    [ContextMenu("Create Simple Gun")]
    void CreateSimpleGun()
    {
        // Clear existing children
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Create gun body (handle)
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handle.name = "Handle";
        handle.transform.SetParent(transform);
        handle.transform.localPosition = Vector3.zero;
        handle.transform.localScale = new Vector3(0.05f, 0.15f, 0.05f);
        handle.transform.localRotation = Quaternion.identity;

        // Create barrel
        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Barrel";
        barrel.transform.SetParent(transform);
        barrel.transform.localPosition = new Vector3(0, 0.1f, 0.1f);
        barrel.transform.localScale = new Vector3(0.03f, 0.1f, 0.03f);
        barrel.transform.localRotation = Quaternion.Euler(90, 0, 0);

        // Create muzzle point
        GameObject muzzle = new GameObject("MuzzlePoint");
        muzzle.transform.SetParent(transform);
        muzzle.transform.localPosition = new Vector3(0, 0.1f, 0.2f);
        muzzle.transform.localRotation = Quaternion.identity;

        // Set color for all parts
        Renderer handleRenderer = handle.GetComponent<Renderer>();
        Renderer barrelRenderer = barrel.GetComponent<Renderer>();

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = visualColor;

        handleRenderer.material = mat;
        barrelRenderer.material = mat;

        // Add Rigidbody to parent
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.mass = 0.5f;
        rb.useGravity = false;
        rb.isKinematic = true;

        // Add collider to handle for grabbing
        BoxCollider handleCollider = handle.GetComponent<BoxCollider>();
        handleCollider.isTrigger = false;

        // Remove colliders from barrel (not needed for interaction)
        DestroyImmediate(barrel.GetComponent<Collider>());

        // Add XRGrabInteractable if not present
        XRGrabInteractable grabInteractable = gameObject.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;

        // Add GunController if not present
        GunController gunController = gameObject.GetComponent<GunController>();
        if (gunController == null)
        {
            gunController = gameObject.AddComponent<GunController>();
        }

        Debug.Log($"Simple {gunColor} gun created! Now assign the bullet prefab in the inspector.");
    }

    [ContextMenu("Set Gun Color - Red")]
    void SetColorRed()
    {
        gunColor = Bullet.BulletColor.Red;
        visualColor = new Color(0.8f, 0.2f, 0.2f);
        CreateSimpleGun();
    }

    [ContextMenu("Set Gun Color - Blue")]
    void SetColorBlue()
    {
        gunColor = Bullet.BulletColor.Blue;
        visualColor = new Color(0.2f, 0.4f, 0.8f);
        CreateSimpleGun();
    }

    [ContextMenu("Set Gun Color - White")]
    void SetColorWhite()
    {
        gunColor = Bullet.BulletColor.White;
        visualColor = new Color(0.9f, 0.9f, 0.9f);
        CreateSimpleGun();
    }
}
