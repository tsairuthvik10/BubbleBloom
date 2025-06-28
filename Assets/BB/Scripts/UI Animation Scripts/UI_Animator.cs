using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UI_Animator : MonoBehaviour
{
    public float animationDuration = 0.3f;
    private CanvasGroup canvasGroup;
    private Coroutine currentAnimation;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void AnimateIn()
    {
        // ** THE FIX **
        // First, ensure the GameObject is active, THEN start the coroutine.
        gameObject.SetActive(true);
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(DoAnimateIn());
    }

    public void AnimateOut()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(DoAnimateOut());
    }

    private IEnumerator DoAnimateIn()
    {
        canvasGroup.blocksRaycasts = true; // Prevent clicking during animation
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        float timer = 0f;
        while (timer < animationDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / animationDuration);
            transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, timer / animationDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator DoAnimateOut()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;

        float timer = 0f;
        while (timer < animationDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / animationDuration);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, timer / animationDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // ** THE FIX **
        // Now, set the object to inactive AFTER the animation has finished.
        gameObject.SetActive(false);
        canvasGroup.blocksRaycasts = false;
    }
}