using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
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
    [SerializeField] private GameObject hat;
    [SerializeField] private Vector2 xLook;
    [SerializeField] private float lookSensitivity;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private Equipment equipment;
    private Interaction interaction;
    private PlayerCondition condition;
    private Condition stamina;
    private Vector2 movementInput;
    private Vector2 mouseDelta;
    private Tween delayedCall;
    private float camCurXRot;
    private float animationSpeed;
    private bool isSprinting;
    private bool isGrounded;
    private int camIndex = 0;

    private const float JumpCooldown = 0.15f;
    private float jumpCooldownTimer;

    public bool CanLook { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        equipment = GetComponent<Equipment>();
        interaction = GetComponent<Interaction>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraContainer.localEulerAngles = Vector3.zero;
        mouseDelta = Vector3.zero;

        condition = CharacterManager.Instance.Player.Condition;
        stamina = condition.UICondition.Stamina;

        hat.SetActive(false);
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
            CanLook = !CanLook;
        }

        float targetWeight = (equipment.CurEquip != null) ? 1f : 0f;
        ChangeLayerWeight(targetWeight);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        if (CanLook)
        {
            HandleCameraLook();
        }
    }

    private void ChangeLayerWeight(float weight)
    {
        float currentWeight = animator.GetLayerWeight(1);

        DOTween.To(() => currentWeight, currentWeight => animator.SetLayerWeight(1, currentWeight), weight, 0.5f)
            .SetEase(Ease.OutCubic);
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
        if (!CanLook) return;

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

            Cinemachine3rdPersonFollow cinemachine3rdPersonFollow = vCams[camIndex].GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            if (cinemachine3rdPersonFollow != null)
            {
                delayedCall.Kill();
                hat.SetActive(true);
                interaction.CheckDistanceBonus = cinemachine3rdPersonFollow.CameraDistance;
            }
            else
            {
                delayedCall = DOVirtual.DelayedCall(mainCam.m_DefaultBlend.BlendTime, () => hat.SetActive(false));
                interaction.CheckDistanceBonus = 0f;
            }
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
        CanLook = !toggle;
    }

    private bool CheckGrounded()
    {
        return Physics.CheckSphere(transform.position + Vector3.up * 0.1f, 0.3f, groundLayerMask, QueryTriggerInteraction.Ignore);
    }
}
