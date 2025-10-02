using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectInfoMonitor : MonoBehaviour
{
    [Header("Controller References")]
    [SerializeField] private XRDirectInteractor leftDirectInteractor;
    [SerializeField] private XRDirectInteractor rightDirectInteractor;

    public GameObject CurrentGrabbedObject { get; private set; }
    public float ObjectMass { get; private set; }
    public Vector3 ObjectSize { get; private set; }
    public float ObjectBounciness { get; private set; }
    public bool IsHoldingObject => CurrentGrabbedObject != null;

    void Start()
    {
        // Subscribe to grab events
        if (leftDirectInteractor != null)
        {
            leftDirectInteractor.selectEntered.AddListener(OnObjectGrabbed);
            leftDirectInteractor.selectExited.AddListener(OnObjectReleased);
        }

        if (rightDirectInteractor != null)
        {
            rightDirectInteractor.selectEntered.AddListener(OnObjectGrabbed);
            rightDirectInteractor.selectExited.AddListener(OnObjectReleased);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (leftDirectInteractor != null)
        {
            leftDirectInteractor.selectEntered.RemoveListener(OnObjectGrabbed);
            leftDirectInteractor.selectExited.RemoveListener(OnObjectReleased);
        }

        if (rightDirectInteractor != null)
        {
            rightDirectInteractor.selectEntered.RemoveListener(OnObjectGrabbed);
            rightDirectInteractor.selectExited.RemoveListener(OnObjectReleased);
        }
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        GameObject grabbedObject = args.interactableObject.transform.gameObject;
        CurrentGrabbedObject = grabbedObject;
        UpdateObjectInfo(grabbedObject);
    }

    private void OnObjectReleased(SelectExitEventArgs args)
    {
        CurrentGrabbedObject = null;
        ObjectMass = 0f;
        ObjectSize = Vector3.zero;
        ObjectBounciness = 0f;
    }

    private void UpdateObjectInfo(GameObject obj)
    {
        // Get mass
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        ObjectMass = rb != null ? rb.mass : 0f;

        // Get size (from collider bounds)
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            ObjectSize = col.bounds.size;
        }
        else
        {
            ObjectSize = Vector3.zero;
        }

        // Get bounciness (from physics material)
        if (col != null && col.material != null)
        {
            ObjectBounciness = col.material.bounciness;
        }
        else
        {
            ObjectBounciness = 0f;
        }
    }

    public string GetObjectInfoString()
    {
        if (!IsHoldingObject)
        {
            return "No object held";
        }

        return $"Mass: {ObjectMass:F2} kg\n" +
               $"Size: {ObjectSize.x:F2} x {ObjectSize.y:F2} x {ObjectSize.z:F2} m\n" +
               $"Bounciness: {ObjectBounciness:F2}";
    }
}
