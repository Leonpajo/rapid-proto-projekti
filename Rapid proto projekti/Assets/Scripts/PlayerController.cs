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

    [Header("Audio")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private AudioClip jumpSound;

    [Header("Interaction / Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask pickupLayerMask = ~0; // what layers count as pickable, default = everything
    [SerializeField] private GameObject interactText;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float cameraPitch;
    private bool controlsEnabled = true;
    private float stepTimer;
    private AudioSource audioSource;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start() // cursor hidden and doesnt exit screen
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        if (!controlsEnabled) return;

        HandleGroundCheck();
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandlePickup();
        HandleCursorToggle();
    }

    private void HandleCursorToggle()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
        }
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
        if (UnityEngine.Cursor.lockState != CursorLockMode.Locked) return;

        Mouse mouse = Mouse.current;
        if (mouse == null || cameraTransform == null) return;

        Vector2 delta = mouse.delta.ReadValue() * mouseSensitivity;

        transform.Rotate(Vector3.up * delta.x);

        cameraPitch -= delta.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
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

            float currentSpeed = kb.leftShiftKey.isPressed
                ? sprintSpeed
                : walkSpeed;

            // Pelaaja liikkuu myös ilmassa
            controller.Move(moveDir * currentSpeed * Time.deltaTime);

            // Askelääni vain maassa
            if (isGrounded)
            {
                stepTimer -= Time.deltaTime;

                if (stepTimer <= 0f)
                {
                    if (footstepSound != null)
                    {
                        audioSource.PlayOneShot(footstepSound);
                    }

                    stepTimer = footstepInterval;
                }
            }
        }
        else
        {
            // Pelaaja ei liiku
            stepTimer = 0f;
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound, 1f);
            }
            else
            {
                Debug.LogWarning("Jump Sound ei ole asetettu!");
            }
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

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        bool lookingAtPickable = Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayerMask)
                                  && hit.collider.GetComponent<Pickable>() != null;

        if (interactText != null)
        {
            interactText.SetActive(lookingAtPickable);
        }

        if (lookingAtPickable && kb.eKey.wasPressedThisFrame)
        {
            Pickable pickable = hit.collider.GetComponent<Pickable>();
            PickUp(pickable);
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
    public void EndGame()
    {
        controlsEnabled = false;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
}