using UnityEngine;
using DG.Tweening;

public abstract class BaseUI : MonoBehaviour
{
    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public virtual void Initialize()
    {
        Debug.Log($"{gameObject.name} Initialized");
    }

    public virtual void OnShow()
    {
        gameObject.SetActive(true);
        PlayShowAnimation();
    }

    public virtual void OnHide()
    {
        PlayHideAnimation();
    }

    protected void PlayShowAnimation()
    {
        // Time.timeScale = 0f;

        canvasGroup.alpha = 0;

        canvasGroup.DOFade(1, 1f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    protected void PlayHideAnimation()
    {
        // Time.timeScale = 1f;

        canvasGroup.DOFade(0, 0.5f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
