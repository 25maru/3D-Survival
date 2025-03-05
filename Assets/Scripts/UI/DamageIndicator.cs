using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private Image image;
    [SerializeField] private float flashSpeed;

    private Vignette vignette;

    private Coroutine coroutine;

    private void Start()
    {
        if (!volume.profile.TryGetSettings(out vignette))
        {
            vignette = volume.profile.AddSettings<Vignette>();
        }
        
        CharacterManager.Instance.Player.Condition.OnTakeDamage += Flash;
    }

    public void Flash()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (image != null)
        {
            image.enabled = true;
            image.color = new Color(1f, 105f / 255f, 105f / 255f);
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

            if (image != null)
            {
                image.color = new Color(1f, 100f / 255f, 100f / 255f, a);
            }
            
            vignette.intensity.value = 0.2f + a;
            vignette.color.value = new Color(3f * startAlpha, 0f, 0f);

            yield return null;
        }

        if (image != null)
        {
            image.enabled = false;
        }

        vignette.intensity.value = 0.2f;
        vignette.color.value = new Color(0f, 0f, 0f);
    }
}