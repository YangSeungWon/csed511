using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectInfoMonitor : MonoBehaviour
{
    [Header("Controller References")]
    [SerializeField] private XRBaseInteractor leftInteractor;
    [SerializeField] private XRBaseInteractor rightInteractor;

    private GameObject leftHoveredObject;
    private GameObject rightHoveredObject;
    private GameObject leftGrabbedObject;
    private GameObject rightGrabbedObject;

    public GameObject CurrentObject { get; private set; }
    public float ObjectMass { get; private set; }
    public Vector3 ObjectSize { get; private set; }
    public float ObjectBounciness { get; private set; }
    public bool IsHoldingObject => leftGrabbedObject != null || rightGrabbedObject != null;

    void Start()
    {
        // Subscribe to grab and hover events
        if (leftInteractor != null)
        {
            leftInteractor.selectEntered.AddListener(OnLeftGrabbed);
            leftInteractor.selectExited.AddListener(OnLeftReleased);
            leftInteractor.hoverEntered.AddListener(OnLeftHoverEntered);
            leftInteractor.hoverExited.AddListener(OnLeftHoverExited);
        }

        if (rightInteractor != null)
        {
            rightInteractor.selectEntered.AddListener(OnRightGrabbed);
            rightInteractor.selectExited.AddListener(OnRightReleased);
            rightInteractor.hoverEntered.AddListener(OnRightHoverEntered);
            rightInteractor.hoverExited.AddListener(OnRightHoverExited);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (leftInteractor != null)
        {
            leftInteractor.selectEntered.RemoveListener(OnLeftGrabbed);
            leftInteractor.selectExited.RemoveListener(OnLeftReleased);
            leftInteractor.hoverEntered.RemoveListener(OnLeftHoverEntered);
            leftInteractor.hoverExited.RemoveListener(OnLeftHoverExited);
        }

        if (rightInteractor != null)
        {
            rightInteractor.selectEntered.RemoveListener(OnRightGrabbed);
            rightInteractor.selectExited.RemoveListener(OnRightReleased);
            rightInteractor.hoverEntered.RemoveListener(OnRightHoverEntered);
            rightInteractor.hoverExited.RemoveListener(OnRightHoverExited);
        }
    }

    void Update()
    {
        UpdateCurrentObject();
    }

    // Left hand events
    private void OnLeftGrabbed(SelectEnterEventArgs args)
    {
        leftGrabbedObject = args.interactableObject.transform.gameObject;
    }

    private void OnLeftReleased(SelectExitEventArgs args)
    {
        leftGrabbedObject = null;
    }

    private void OnLeftHoverEntered(HoverEnterEventArgs args)
    {
        leftHoveredObject = args.interactableObject.transform.gameObject;
    }

    private void OnLeftHoverExited(HoverExitEventArgs args)
    {
        leftHoveredObject = null;
    }

    // Right hand events
    private void OnRightGrabbed(SelectEnterEventArgs args)
    {
        rightGrabbedObject = args.interactableObject.transform.gameObject;
    }

    private void OnRightReleased(SelectExitEventArgs args)
    {
        rightGrabbedObject = null;
    }

    private void OnRightHoverEntered(HoverEnterEventArgs args)
    {
        rightHoveredObject = args.interactableObject.transform.gameObject;
    }

    private void OnRightHoverExited(HoverExitEventArgs args)
    {
        rightHoveredObject = null;
    }

    private void UpdateCurrentObject()
    {
        GameObject newCurrentObject = null;

        // Priority: Right grabbed > Left grabbed > Right hovered > Left hovered
        if (rightGrabbedObject != null)
        {
            newCurrentObject = rightGrabbedObject;
        }
        else if (leftGrabbedObject != null)
        {
            newCurrentObject = leftGrabbedObject;
        }
        else if (rightHoveredObject != null)
        {
            newCurrentObject = rightHoveredObject;
        }
        else if (leftHoveredObject != null)
        {
            newCurrentObject = leftHoveredObject;
        }

        // Update info only if object changed
        if (newCurrentObject != CurrentObject)
        {
            CurrentObject = newCurrentObject;
            if (CurrentObject != null)
            {
                UpdateObjectInfo(CurrentObject);
            }
            else
            {
                ClearObjectInfo();
            }
        }
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

    private void ClearObjectInfo()
    {
        ObjectMass = 0f;
        ObjectSize = Vector3.zero;
        ObjectBounciness = 0f;
    }

    public string GetObjectInfoString()
    {
        if (CurrentObject == null)
        {
            return "No object";
        }

        return $"{CurrentObject.name}\n" +
               $"Mass: {ObjectMass:F2} kg\n" +
               $"Size: {ObjectSize.x:F2} x {ObjectSize.y:F2} x {ObjectSize.z:F2} m\n" +
               $"Bounciness: {ObjectBounciness:F2}";
    }

    // For backwards compatibility
    public GameObject CurrentGrabbedObject => leftGrabbedObject != null ? leftGrabbedObject : rightGrabbedObject;
}
