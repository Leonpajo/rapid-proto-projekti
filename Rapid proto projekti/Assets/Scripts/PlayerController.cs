using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Look/Sens")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 85f; // camera cant flip upside down

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float groundedGravity = -2f; // improvement for ground check

    [Header("Interaction / Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask pickupLayerMask = ~0; // what layers count as pickable, default = everything

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float cameraPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start() // cursor hidden and doesnt exit screen
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandlePickup();
    }

    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = groundedGravity;
        }
    }

    private void HandleMouseLook()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || cameraTransform == null) return;

        Vector2 delta = mouse.delta.ReadValue() * mouseSensitivity;

        transform.Rotate(Vector3.up * delta.x);


        cameraPitch -= delta.y; // cant flip the character upside down
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);  // clamp so cant look too mu
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f); // moves camera
    }

    private void HandleMovement()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        float horizontal = 0f;
        float vertical = 0f;

        if (kb.aKey.isPressed) horizontal -= 1f;
        if (kb.dKey.isPressed) horizontal += 1f;
        if (kb.sKey.isPressed) vertical -= 1f;
        if (kb.wKey.isPressed) vertical += 1f;

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 moveDir = transform.TransformDirection(inputDir);
            float currentSpeed = kb.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // v = sqrt(h * -2 * g), AI 
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandlePickup()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || cameraTransform == null) return;

        if (!kb.eKey.wasPressedThisFrame) return; // press E to pick up

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayerMask))
        {
            Pickable pickable = hit.collider.GetComponent<Pickable>();
            if (pickable != null)
            {
                PickUp(pickable);
            }
        }
    }

    private void PickUp(Pickable pickable)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BookFound(pickable.bookID);
        }

        if (pickable.foundSound != null)
        {
            AudioSource.PlayClipAtPoint(pickable.foundSound, pickable.transform.position);
        }

        Destroy(pickable.gameObject);
    }
}