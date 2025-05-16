using System.Collections;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("UIFader needs a CanvasGroup!");
        }
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(0, 1));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(1, 0));
    }

    private IEnumerator FadeCanvas(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (endAlpha == 0)
            gameObject.SetActive(false);
    }
}
