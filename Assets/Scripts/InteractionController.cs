using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private LayerMask interactableLayer = -1;
    [SerializeField] private Transform holdPosition;

    private Camera playerCamera;
    private GameObject currentHeldObject;
    private Rigidbody heldObjectRigidbody;
    private Collider heldObjectCollider;
    private bool isHolding = false;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        
        GameObject holdPosObject = new GameObject("Hold Position");
        holdPosObject.transform.parent = playerCamera.transform;
        holdPosObject.transform.localPosition = new Vector3(0, 0, holdDistance);
        holdPosition = holdPosObject.transform;
    }

    void Update()
    {
        HandlePickupDrop();
        HandleThrow();
        UpdateHeldObjectPosition();
        ShowInteractionHint();
    }

    void HandlePickupDrop()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    void HandleThrow()
    {
        if (Input.GetMouseButtonDown(1) && isHolding)
        {
            ThrowObject();
        }
    }

    void TryPickupObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Pickupable") || hitObject.GetComponent<Rigidbody>() != null)
            {
                currentHeldObject = hitObject;
                heldObjectRigidbody = currentHeldObject.GetComponent<Rigidbody>();
                heldObjectCollider = currentHeldObject.GetComponent<Collider>();

                if (heldObjectRigidbody == null)
                {
                    heldObjectRigidbody = currentHeldObject.AddComponent<Rigidbody>();
                }

                heldObjectRigidbody.useGravity = false;
                heldObjectRigidbody.drag = 10;  // linearDamping -> drag
                heldObjectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                // 들고 있는 물체를 Ignore Raycast 레이어로 변경
                currentHeldObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                if (heldObjectCollider != null)
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), heldObjectCollider, true);
                }

                isHolding = true;
            }
        }
    }

    void DropObject()
    {
        if (currentHeldObject == null) return;

        heldObjectRigidbody.useGravity = true;
        heldObjectRigidbody.drag = 1;  // linearDamping -> drag
        heldObjectRigidbody.constraints = RigidbodyConstraints.None;

        // 물체를 원래 레이어로 되돌림 (Default)
        currentHeldObject.layer = LayerMask.NameToLayer("Default");

        if (heldObjectCollider != null && GetComponent<Collider>() != null)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), heldObjectCollider, false);
        }

        currentHeldObject = null;
        heldObjectRigidbody = null;
        heldObjectCollider = null;
        isHolding = false;
    }

    void ThrowObject()
    {
        if (currentHeldObject == null) return;

        heldObjectRigidbody.useGravity = true;
        heldObjectRigidbody.drag = 1;  // linearDamping -> drag
        heldObjectRigidbody.constraints = RigidbodyConstraints.None;

        // 물체를 원래 레이어로 되돌림 (Default)
        currentHeldObject.layer = LayerMask.NameToLayer("Default");

        if (heldObjectCollider != null && GetComponent<Collider>() != null)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), heldObjectCollider, false);
        }

        heldObjectRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);

        currentHeldObject = null;
        heldObjectRigidbody = null;
        heldObjectCollider = null;
        isHolding = false;
    }

    void UpdateHeldObjectPosition()
    {
        if (!isHolding || currentHeldObject == null) return;

        Vector3 targetPosition = holdPosition.position;
        Vector3 direction = targetPosition - currentHeldObject.transform.position;
        heldObjectRigidbody.velocity = direction * 10f;  // linearVelocity -> velocity

        currentHeldObject.transform.rotation = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0);
    }

    void ShowInteractionHint()
    {
        if (!isHolding)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Pickupable") || hitObject.GetComponent<Rigidbody>() != null)
                {
                    Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange, Color.green);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerCamera.transform.position, interactionRange);
        }
    }
}