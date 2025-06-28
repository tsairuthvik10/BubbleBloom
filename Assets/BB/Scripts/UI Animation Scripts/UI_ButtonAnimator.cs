using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UI_ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 initialScale;
    private bool isPointerDown = false;

    void Awake()
    {
        initialScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable)
        {
            isPointerDown = true;
            StopAllCoroutines();
            StartCoroutine(AnimateScale(initialScale * 0.9f));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerDown)
        {
            isPointerDown = false;
            StopAllCoroutines();
            StartCoroutine(AnimateScale(initialScale));
        }
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        float duration = 0.15f;
        float timer = 0f;
        Vector3 startScale = transform.localScale;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}