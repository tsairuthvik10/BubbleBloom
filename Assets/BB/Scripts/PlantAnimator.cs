using UnityEngine;
using System.Collections;

public class PlantAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float growthDuration = 0.7f;
    public float overshootAmount = 1.2f;

    void Start()
    {
        StartCoroutine(AnimateGrowth());
    }

    private IEnumerator AnimateGrowth()
    {
        Vector3 finalScale = transform.localScale;
        Vector3 overshootScale = finalScale * overshootAmount;
        transform.localScale = Vector3.zero;

        float timer = 0f;
        while (timer < growthDuration / 2)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, overshootScale, timer / (growthDuration / 2));
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < growthDuration / 2)
        {
            transform.localScale = Vector3.Lerp(overshootScale, finalScale, timer / (growthDuration / 2));
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = finalScale;
    }
}