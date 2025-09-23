using UnityEngine;

public class TeleportationController : MonoBehaviour
{
    [Header("Teleportation Settings")]
    [SerializeField] private float maxTeleportDistance = 20f;
    [SerializeField] private LayerMask teleportLayer = -1;
    [SerializeField] private GameObject teleportIndicatorPrefab;
    [SerializeField] private LineRenderer teleportLine;
    [SerializeField] private Color validTeleportColor = Color.green;
    [SerializeField] private Color invalidTeleportColor = Color.red;

    private Camera playerCamera;
    private GameObject teleportIndicator;
    private bool canTeleport = false;
    private Vector3 teleportDestination;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        CreateTeleportIndicator();
    }

    void CreateTeleportIndicator()
    {
        if (teleportIndicatorPrefab == null)
        {
            teleportIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            teleportIndicator.name = "Teleport Indicator";
            teleportIndicator.transform.localScale = new Vector3(1f, 0.1f, 1f);

            Renderer renderer = teleportIndicator.GetComponent<Renderer>();
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = new Color(0, 1, 0, 0.5f);  // 녹색 반투명
            
            // URP 투명도 설정
            material.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
            material.SetFloat("_Blend", 0);   // 0 = Alpha, 1 = Premultiply, 2 = Additive, 3 = Multiply
            material.renderQueue = 3000;
            renderer.material = material;

            Collider collider = teleportIndicator.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
        }
        else
        {
            teleportIndicator = Instantiate(teleportIndicatorPrefab);
        }

        teleportIndicator.SetActive(false);
    }



    void Update()
    {
        ShowTeleportPreview();

        if (Input.GetMouseButtonDown(0) && canTeleport)
        {
            PerformTeleport();
        }
    }

    void ShowTeleportPreview()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxTeleportDistance, teleportLayer))
        {
            canTeleport = true;
            teleportDestination = hit.point;

            teleportIndicator.SetActive(true);
            teleportIndicator.transform.position = teleportDestination + Vector3.up * 0.05f;

            Renderer indicatorRenderer = teleportIndicator.GetComponent<Renderer>();
            if (indicatorRenderer != null)
            {
                indicatorRenderer.material.color = new Color(validTeleportColor.r, validTeleportColor.g, validTeleportColor.b, 0.5f);
            }
        }
        else
        {
            canTeleport = false;
            teleportIndicator.SetActive(false);
        }
    }



    void PerformTeleport()
    {
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = new Vector3(teleportDestination.x, teleportDestination.y + 0.1f, teleportDestination.z);
            characterController.enabled = true;
        }
        else
        {
            transform.position = new Vector3(teleportDestination.x, teleportDestination.y + 0.1f, teleportDestination.z);
        }

        teleportIndicator.SetActive(false);
        canTeleport = false;
    }
}