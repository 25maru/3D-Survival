using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private float flashSpeed;

    private PlayerCondition playerCondition;
    private Coroutine coroutine;

    private void Start()
    {
        playerCondition = CharacterManager.Instance.Player.Condition;
        playerCondition.OnTakeDamage += Flash;
        playerCondition.OnDeath += HandleDeath;
    }

    public void Flash()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
        coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0.0f)
        {
            a -= startAlpha / flashSpeed * Time.deltaTime;

            var health = playerCondition.UICondition.Health;
            var percentage = health.GetPercentage();
            var weight = (1f - percentage) / 2f + 0.5f;

            volume.weight = 3.3f * weight * a;

            yield return null;
        }

        volume.weight = 0f;
    }

    public void HandleDeath()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        volume.weight = 1f;
    }
}