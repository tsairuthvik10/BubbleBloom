using UnityEngine;
using System.Collections;
using System;

public class Bubble : MonoBehaviour
{
    [Header("Settings")]
    public float activationTime = 2.0f;
    public float chainRadius = 5.0f;

    [Header("Visuals & Prefabs")]
    public Material defaultMaterial;
    public Material activatedMaterial;
    public GameObject chainLinkPrefab;
    public GameObject impactVFXPrefab; // Optional: A small "spark" particle effect

    private bool isActivated = false;
    private Renderer rend;
    private bool hasBeenPopped = false;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null) rend.material = defaultMaterial;
    }

    public void Pop()
    {
        if (hasBeenPopped) return;
        hasBeenPopped = true;

        int pointsGained = 0;
        if (GameManager.Instance != null)
        {
            pointsGained = GameManager.Instance.OnBubblePopped(transform.position);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowFloatingScore(pointsGained, transform.position);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayBubblePop();
        if (VFXManager.Instance != null) VFXManager.Instance.PlayBubblePopVFX(transform.position);

        Collider[] bubblesInRange = Physics.OverlapSphere(transform.position, chainRadius);
        foreach (var col in bubblesInRange)
        {
            if (col.CompareTag("Bubble") && col.gameObject != this.gameObject)
            {
                Bubble nearbyBubble = col.GetComponent<Bubble>();
                if (nearbyBubble != null && !nearbyBubble.isActivated)
                {
                    nearbyBubble.Activate(transform.position);
                }
            }
        }

        Destroy(gameObject);
    }

    public void Activate(Vector3 activatorPosition)
    {
        if (isActivated) return;
        isActivated = true;
        if (rend != null) rend.material = activatedMaterial;

        if (chainLinkPrefab != null)
        {
            StartCoroutine(AnimateChainLink(activatorPosition, transform.position));
        }

        StartCoroutine(ActivationTimer());
    }

    // ** NEW, IMPROVED ANIMATION **
    private IEnumerator AnimateChainLink(Vector3 startPos, Vector3 endPos)
    {
        GameObject linkGO = Instantiate(chainLinkPrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = linkGO.GetComponent<LineRenderer>();

        float timer = 0f;
        // ** THE FIX: Increased duration for a more noticeable effect **
        float duration = 0.4f;

        Vector3 pos1 = startPos;
        Vector3 pos2 = startPos;

        lr.SetPosition(0, pos1);
        lr.SetPosition(1, pos2);

        // This makes the trail "shoot" across
        while (timer < duration)
        {
            pos2 = Vector3.Lerp(startPos, endPos, timer / duration);
            lr.SetPosition(1, pos2);

            // This makes the tail of the trail follow the head
            float tailDelay = 0.15f; // Adjusted for new duration
            if (timer > tailDelay)
            {
                pos1 = Vector3.Lerp(startPos, endPos, (timer - tailDelay) / duration);
                lr.SetPosition(0, pos1);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Play an impact effect when the trail "hits" the target bubble
        if (impactVFXPrefab != null)
        {
            Instantiate(impactVFXPrefab, endPos, Quaternion.identity);
        }

        Destroy(linkGO); // The trail disappears immediately after hitting
    }

    private IEnumerator ActivationTimer()
    {
        yield return new WaitForSeconds(activationTime);
        Deactivate();
    }

    private void Deactivate()
    {
        isActivated = false;
        if (rend != null && !hasBeenPopped) rend.material = defaultMaterial;
    }
}