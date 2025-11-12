using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

/// <summary>
/// VR Character Controller with jump mechanics for Fall Guys game
/// Extends XR Toolkit movement with gravity and jump functionality
/// </summary>
public class VRCharacterController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Controller References")]
    [SerializeField] private ActionBasedController rightController;
    [SerializeField] private InputActionReference jumpAction;

    private CharacterController characterController;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 1, 0);
            characterController.radius = 0.3f;
            characterController.height = 1.8f;
        }
    }

    void OnEnable()
    {
        if (jumpAction != null && jumpAction.action != null)
        {
            jumpAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (jumpAction != null && jumpAction.action != null)
        {
            jumpAction.action.Disable();
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleJump();
        ApplyGravity();
    }

    /// <summary>
    /// Check if player is on the ground
    /// </summary>
    private void CheckGrounded()
    {
        Vector3 rayStart = transform.position;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);

        // Reset vertical velocity when grounded
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to keep grounded
        }
    }

    /// <summary>
    /// Handle jump input
    /// </summary>
    private void HandleJump()
    {
        // Check if jump button was pressed (B button on controller)
        bool jumpPressed = false;

        if (jumpAction != null && jumpAction.action != null)
        {
            jumpPressed = jumpAction.action.WasPressedThisFrame();
        }

        if (jumpPressed && isGrounded)
        {
            verticalVelocity = jumpForce;
            Debug.Log("Jump!");
        }
    }

    /// <summary>
    /// Apply gravity to character
    /// </summary>
    private void ApplyGravity()
    {
        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Apply vertical movement
        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        characterController.Move(verticalMove);
    }

    /// <summary>
    /// Get current grounded state
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// Apply external force (for knockback)
    /// </summary>
    public void ApplyForce(Vector3 force)
    {
        characterController.Move(force * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        // Draw ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
