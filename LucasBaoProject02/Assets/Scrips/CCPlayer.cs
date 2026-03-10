using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CCPlayer : MonoBehaviour
{
    public float walkSpeed = 5;
    public float runSpeed = 9;
    public float jumpHeight = 5;

    public Transform cameraTransform;
    public float lookSensitivity = 1f;

    private CharacterController cc;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float verticalVelocity;
    private float gravity = -20f;

    private float pitch;

    [SerializeField] private Image reticleImage;

    private bool interactPressed;

    public static event Action<NPCData> OnDialogueRequested;

    private Interactable currentInteractable;

    private bool isRunning;
    private bool isJumping;

    public bool controlsLocked;

    // ===== 移动平台变量 =====
    private MovingPlatform currentPlatform;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (reticleImage == null)
        {
            GameObject reticle = GameObject.Find("Reticle");

            if (reticle != null)
                reticleImage = reticle.GetComponent<Image>();
        }

        if (reticleImage != null)
            reticleImage.color = new Color(0, 0, 0, .7f);
    }

    void Update()
    {
        if (cameraTransform == null) return;

        if (!controlsLocked)
        {
            HandleLook();
            HandleMovement();
            CheckInteract();
            HandleInteract();
        }
    }

    private void HandleLook()
    {
        float yaw = lookInput.x * lookSensitivity;
        float pitchDelta = lookInput.y * lookSensitivity;

        transform.Rotate(Vector3.up * yaw);

        pitch -= pitchDelta;
        pitch = Mathf.Clamp(pitch, -90, 90);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void HandleMovement()
    {
        bool grounded = cc.isGrounded;

        if (grounded && verticalVelocity <= 0)
        {
            verticalVelocity = -2f;
        }

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 move =
            transform.right * moveInput.x * currentSpeed +
            transform.forward * moveInput.y * currentSpeed;

        if (isJumping && grounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            isJumping = false;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = Vector3.up * verticalVelocity;

        // ===== 玩家移动 =====
        cc.Move((move + velocity) * Time.deltaTime);
    }

    // ===== 检测移动平台 =====
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();

        if (platform != null)
        {
            currentPlatform = platform;
        }
        else
        {
            currentPlatform = null;
        }
    }

    // ===== 平台带动玩家 =====
    void LateUpdate()
    {
        if (currentPlatform != null)
        {
            cc.Move(currentPlatform.platformVelocity * Time.deltaTime);
        }
    }

    void CheckInteract()
    {
        if (reticleImage != null)
            reticleImage.color = new Color(0, 0, 0, .7f);

        currentInteractable = null;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            currentInteractable = hit.collider.GetComponentInParent<Interactable>();

            if (currentInteractable != null && reticleImage != null)
                reticleImage.color = Color.red;
        }
    }

    void HandleInteract()
    {
        if (!interactPressed) return;

        interactPressed = false;

        if (currentInteractable == null) return;

        currentInteractable.Interact(this);
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            isJumping = true;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log("Interact Pressed");
        interactPressed = true;
    }

    public void RequestDialogue(NPCData npcData)
    {
        OnDialogueRequested?.Invoke(npcData);
    }

    public void SetControlsLocked(bool locked)
    {
        controlsLocked = locked;

        if (locked)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;

            verticalVelocity = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}