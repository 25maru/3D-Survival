using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Interaction : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private float checkRate = 0.05f;
    private float lastCheckTime;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI promptTextPlus;

    private GameObject curInteractGameObject;
    private IInteractable curInteractable;
    private RectTransform promptTextRect;
    private CanvasGroup promptTextCanvas;
    private Camera cam;

    public float CheckDistanceBonus { get; set;}

    private void Start()
    {
        promptTextRect = promptText.GetComponent<RectTransform>();
        promptTextCanvas = promptText.GetComponent<CanvasGroup>();
        cam = Camera.main;

        promptTextCanvas.alpha = 0f;
        promptTextRect.anchoredPosition = new(0f, 25f);
    }

    private void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit, maxCheckDistance + CheckDistanceBonus, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    if (curInteractGameObject != null)
                    {
                        curInteractGameObject.GetComponent<IInteractable>().SetOutline(false);
                    }

                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    curInteractable.SetOutline(true);
                    AnimatePromptText(true);
                    SetPromptText();
                }
            }
            else
            {
                if (curInteractGameObject == null && curInteractable == null) return;

                curInteractable.SetOutline(false);
                AnimatePromptText(false);

                curInteractGameObject = null;
                curInteractable = null;
            }
        }
    }

    private void SetPromptText()
    {
        promptText.text = curInteractable.GetInteractPrompt();
        promptTextPlus.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            if (!curInteractable.OnInteract()) return;

            curInteractGameObject = null;
            curInteractable = null;
            AnimatePromptText(false);
        }
    }

    private void AnimatePromptText(bool show)
    {
        float alpha = show ? 1f : 0f;
        float yPos = show ? 50f : 25f;

        promptTextCanvas.DOFade(alpha, 0.5f)
            // .From(1f - alpha)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        promptTextRect.DOAnchorPos(new Vector2(0f, yPos), 0.5f)
            // .From(new Vector2(0f, 75f - yPos))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }
}