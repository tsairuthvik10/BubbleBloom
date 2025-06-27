using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour
{
    [Header("Settings")]
    public float activationTime = 2.0f;
    public float chainRadius = 5.0f;

    [Header("Visuals")]
    public Material defaultMaterial;
    public Material activatedMaterial;

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
        if (!isActivated && GameManager.Instance != null && GameManager.Instance.currentCombo > 0) return;

        hasBeenPopped = true;

        if (GameManager.Instance != null) GameManager.Instance.OnBubblePopped(transform.position);
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
                    nearbyBubble.Activate();
                }
            }
        }

        CancelInvoke("ComboTimeout");
        Destroy(gameObject);
    }

    public void Activate()
    {
        if (isActivated) return;
        isActivated = true;
        if (rend != null) rend.material = activatedMaterial;
        StartCoroutine(ActivationTimer());
        Invoke("ComboTimeout", activationTime + 0.5f);
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

    private void ComboTimeout()
    {
        if (GameManager.Instance != null) GameManager.Instance.ResetCombo();
    }
}