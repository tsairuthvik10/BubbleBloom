using UnityEngine;
using TMPro;
using System.Collections;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Toast Notification")]
    public GameObject toastPanel;
    public TextMeshProUGUI toastText;
    public float toastDuration = 2.0f;

    [Header("Haptics Settings")]
    public bool hapticsEnabled = true;

    private Coroutine currentToastCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (toastPanel != null) toastPanel.SetActive(false);
    }

    public void TriggerHapticFeedback()
    {
        if (!hapticsEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void ShowToast(string message)
    {
        if (toastPanel == null) return;
        if (currentToastCoroutine != null) StopCoroutine(currentToastCoroutine);
        currentToastCoroutine = StartCoroutine(ToastCoroutine(message));
    }

    private IEnumerator ToastCoroutine(string message)
    {
        toastText.text = message;
        toastPanel.SetActive(true);
        yield return new WaitForSeconds(toastDuration);
        toastPanel.SetActive(false);
    }
}
