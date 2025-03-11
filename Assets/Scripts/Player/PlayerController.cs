using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Action inventory;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Look")]
    [SerializeField] private CinemachineBrain mainCam;
    [SerializeField] private List<CinemachineVirtualCamera> vCams;
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private Vector2 xLook;
    [SerializeField] private float lookSensitivity;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private PlayerCondition condition;
    private Condition stamina;
    private Vector2 movementInput;
    private Vector2 mouseDelta;
    private float camCurXRot;
    private float animationSpeed;
    private bool isSprinting;
    private bool isGrounded;
    private int camIndex = 0;

    private const float JumpCooldown = 0.15f;
    private float jumpCooldownTimer;

    private bool canLook = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraContainer.localEulerAngles = Vector3.zero;
        mouseDelta = Vector3.zero;

        condition = CharacterManager.Instance.Player.Condition;
        stamina = condition.UICondition.Stamina;
    }

    private void Update()
    {
        isGrounded = CheckGrounded();

        animator.SetBool("Grounded", isGrounded);

        if (isGrounded)
        {
            animator.SetBool("FreeFall", false);

            if (jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (jumpCooldownTimer <= 0)
            {
                animator.SetBool("FreeFall", true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canLook = !canLook;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            HandleCameraLook();
        }
    }

    private void HandleMovement()
    {
        float boost = condition.SpeedBoost;
        float targetSpeed = movementInput == Vector2.zero ? 0f : ((isSprinting ? sprintSpeed : moveSpeed) + boost);

        Vector3 moveDirection = (transform.forward * movementInput.y + transform.right * movementInput.x).normalized * targetSpeed;
        moveDirection.y = rb.velocity.y;

        rb.velocity = moveDirection;

        animationSpeed = Mathf.Lerp(animationSpeed, targetSpeed, Time.fixedDeltaTime * 10f);
        animator.SetFloat("Speed", animationSpeed);

        if (isSprinting)
        {
            if (stamina.CurValue > 0)
            {
                if (movementInput != Vector2.zero) stamina.Subtract(30f * Time.fixedDeltaTime);
            }
            else
            {
                isSprinting = false;
            }
        }

        mouseDelta = Vector2.Lerp(mouseDelta, Vector2.zero, Time.deltaTime * 10f);
    }

    private void HandleCameraLook()
    {
        if (!canLook) return;

        transform.Rotate(lookSensitivity * mouseDelta.x * Vector3.up);
        camCurXRot -= mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, xLook.x, xLook.y);
        cameraContainer.localEulerAngles = new Vector3(camCurXRot, 0, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && isGrounded && jumpCooldownTimer <= 0)
        {
            jumpCooldownTimer = JumpCooldown;

            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (stamina.CurValue > 20f)
            {
                isSprinting = true;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSprinting = false;
        }
    }

    public void OnToggleViewInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && vCams.Count > 0 && !mainCam.IsBlending)
        {
            vCams[camIndex].Priority = 0;

            camIndex = (camIndex + 1) % vCams.Count;

            vCams[camIndex].Priority = 10;
        }
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    private bool CheckGrounded()
    {
        return Physics.CheckSphere(transform.position + Vector3.up * 0.1f, 0.3f, groundLayerMask, QueryTriggerInteraction.Ignore);
    }
}
