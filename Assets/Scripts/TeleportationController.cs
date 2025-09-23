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
        CreateTeleportLine();
    }

    void CreateTeleportIndicator()
    {
        if (teleportIndicatorPrefab == null)
        {
            teleportIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            teleportIndicator.name = "Teleport Indicator";
            teleportIndicator.transform.localScale = new Vector3(1f, 0.1f, 1f);

            Renderer renderer = teleportIndicator.GetComponent<Renderer>();
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(0, 1, 0, 0.5f);
            material.SetFloat("_Mode", 3);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
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

    void CreateTeleportLine()
    {
        if (teleportLine == null)
        {
            GameObject lineObject = new GameObject("Teleport Line");
            lineObject.transform.parent = transform;
            teleportLine = lineObject.AddComponent<LineRenderer>();

            Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
            teleportLine.material = lineMaterial;
            teleportLine.startWidth = 0.05f;
            teleportLine.endWidth = 0.05f;
            teleportLine.positionCount = 2;
        }

        teleportLine.enabled = false;
    }

    void Update()
    {
        ShowTeleportPreview();

        if (Input.GetMouseButtonDown(0) && canTeleport && Cursor.lockState != CursorLockMode.Locked)
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

            teleportLine.enabled = true;
            teleportLine.SetPosition(0, playerCamera.transform.position);
            teleportLine.SetPosition(1, teleportDestination);
            teleportLine.startColor = validTeleportColor;
            teleportLine.endColor = validTeleportColor;

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
            teleportLine.enabled = false;
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
        teleportLine.enabled = false;
        canTeleport = false;
    }
}