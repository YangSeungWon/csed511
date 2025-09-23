using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Texture2D crosshairTexture;
    [SerializeField] private int crosshairSize = 20;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color interactableColor = Color.green;

    private Color currentColor;
    private InteractionController interactionController;
    private Camera playerCamera;

    void Start()
    {
        interactionController = GetComponent<InteractionController>();
        playerCamera = GetComponentInChildren<Camera>();
        currentColor = normalColor;

        // 기본 크로스헤어 텍스처 생성 (없을 경우)
        if (crosshairTexture == null)
        {
            crosshairTexture = new Texture2D(2, 2);
            crosshairTexture.SetPixel(0, 0, Color.white);
            crosshairTexture.SetPixel(1, 0, Color.white);
            crosshairTexture.SetPixel(0, 1, Color.white);
            crosshairTexture.SetPixel(1, 1, Color.white);
            crosshairTexture.Apply();
        }
    }

    void Update()
    {
        // 집을 수 있는 물체를 보고 있는지 체크
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Pickupable"))
            {
                currentColor = interactableColor;
            }
            else
            {
                currentColor = normalColor;
            }
        }
        else
        {
            currentColor = normalColor;
        }
    }

    void OnGUI()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        GUI.color = currentColor;

        // 화면 중앙에 크로스헤어 그리기
        float xPos = (Screen.width - crosshairSize) / 2;
        float yPos = (Screen.height - crosshairSize) / 2;

        // + 모양 크로스헤어
        GUI.DrawTexture(new Rect(xPos, Screen.height / 2 - 1, crosshairSize, 2), crosshairTexture); // 가로선
        GUI.DrawTexture(new Rect(Screen.width / 2 - 1, yPos, 2, crosshairSize), crosshairTexture); // 세로선
    }
}