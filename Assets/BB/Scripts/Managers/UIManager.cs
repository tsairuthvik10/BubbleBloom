using UnityEngine;
using TMPro;
using Firebase.Firestore; // Assuming you'll use this for leaderboard
using System.Collections;
using UnityEngine.UI; // Required for LayoutRebuilder

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject hudPanel;
    public GameObject summaryPanel;
    public GameObject leaderboardPanel;
    public GameObject profileCreationPanel;

    // We get the animator components from the panels
    private UI_Animator summaryAnimator;
    private UI_Animator leaderboardAnimator;

    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI timerText;

    [Header("Summary")]
    public TextMeshProUGUI summaryScoreText;
    public TextMeshProUGUI summaryComboText;

    [Header("Leaderboard")]
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContentArea;
    public TMP_Dropdown leaderboardSortDropdown;

    [Header("Feedback")]
    public GameObject floatingTextPrefab; // Assign your new FloatingText_Prefab
    public Canvas mainCanvas; // Assign your main UI Canvas

    private Coroutine scoreTextCoroutine;
    private Coroutine comboTextCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        // Get the animator components from the panels if they exist
        if (summaryPanel != null) summaryAnimator = summaryPanel.GetComponent<UI_Animator>();
        if (leaderboardPanel != null) leaderboardAnimator = leaderboardPanel.GetComponent<UI_Animator>();
    }

    void Start()
    {
        if (leaderboardSortDropdown != null)
        {
            leaderboardSortDropdown.onValueChanged.AddListener(delegate { OnSortChanged(); });
        }

        // Hide panels on start without animation
        if (summaryPanel != null) summaryPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
            if (scoreTextCoroutine != null) StopCoroutine(scoreTextCoroutine);
            scoreTextCoroutine = StartCoroutine(AnimateScoreText());
        }
    }

    private IEnumerator AnimateScoreText()
    {
        float duration = 0.25f;
        float timer = 0;
        while (timer < duration / 2)
        {
            scoreText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.3f, timer / (duration / 2));
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        while (timer < duration / 2)
        {
            scoreText.transform.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, timer / (duration / 2));
            timer += Time.deltaTime;
            yield return null;
        }
        scoreText.transform.localScale = Vector3.one;
    }

    public void UpdateCombo(int combo)
    {
        if (comboText == null) return;

        comboText.gameObject.SetActive(combo > 1);
        if (combo > 1)
        {
            comboText.text = $"x{combo} Combo!";
            if (comboTextCoroutine != null) StopCoroutine(comboTextCoroutine);
            comboTextCoroutine = StartCoroutine(ComboTextAnimation());
        }
    }

    private IEnumerator ComboTextAnimation()
    {
        comboText.transform.localScale = Vector3.one * 1.5f;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4f;
            comboText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);
            yield return null;
        }
    }

    public void UpdateTimer(float time)
    {
        if (timerText != null)
        {
            timerText.text = time > 0 ? $"Time: {Mathf.CeilToInt(time)}" : "Time: 0";
        }
    }

    public void ShowFloatingScore(int points, Vector3 worldPosition)
    {
        if (floatingTextPrefab == null || mainCanvas == null) return;

        GameObject textGO = Instantiate(floatingTextPrefab, mainCanvas.transform);

        TextMeshProUGUI tmpText = textGO.GetComponent<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = $"+{points}";
        }

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        screenPosition.x += Random.Range(-20f, 20f);
        screenPosition.y += Random.Range(-20f, 20f);
        textGO.transform.position = screenPosition;
    }

    public void ShowSummaryPanel()
    {
        if (summaryPanel == null) return;

        int finalScore = GameManager.Instance.currentScore;
        int longestCombo = GameManager.Instance.GetSessionLongestCombo();
        if (summaryScoreText != null) summaryScoreText.text = $"Final Score: {finalScore}";
        if (summaryComboText != null) summaryComboText.text = $"Best Combo: {longestCombo}";

        if (hudPanel != null) hudPanel.SetActive(false);

        if (summaryAnimator != null)
        {
            summaryAnimator.AnimateIn();
        }
        else
        {
            summaryPanel.SetActive(true);
        }
    }

    public void OnViewLeaderboardClicked()
    {
        if (leaderboardPanel == null) return;

        if (summaryAnimator != null) summaryAnimator.AnimateOut();
        if (leaderboardAnimator != null) leaderboardAnimator.AnimateIn();

        OnSortChanged();
    }

    public void OnCloseLeaderboardClicked()
    {
        if (leaderboardPanel == null) return;

        if (leaderboardAnimator != null) leaderboardAnimator.AnimateOut();
        if (summaryAnimator != null) summaryAnimator.AnimateIn();
    }

    private async void OnSortChanged()
    {
        if (!leaderboardPanel.activeSelf || LeaderboardManager.Instance == null) return;

        string sortByField = "highestScore";
        if (leaderboardSortDropdown != null)
        {
            switch (leaderboardSortDropdown.value)
            {
                case 1: sortByField = "longestCombo"; break;
                case 2: sortByField = "totalPlantsBloomed"; break;
            }
        }

        var snapshot = await LeaderboardManager.Instance.FetchLeaderboard(sortByField);
        DisplayLeaderboard(snapshot, sortByField);
    }

    private void DisplayLeaderboard(QuerySnapshot snapshot, string statField)
    {
        foreach (Transform child in leaderboardContentArea) Destroy(child.gameObject);
        if (snapshot == null) return;

        int rank = 1;
        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            if (document.Exists)
            {
                GameObject entryGO = Instantiate(leaderboardEntryPrefab, leaderboardContentArea);
                LeaderboardEntryUI entryUI = entryGO.GetComponent<LeaderboardEntryUI>();

                if (entryUI == null)
                {
                    Debug.LogError("LeaderboardEntry_Prefab is missing the LeaderboardEntryUI script!");
                    continue;
                }

                PlayerStats stats = document.ConvertTo<PlayerStats>();

                string statValue = "N/A";
                if (document.ContainsField(statField))
                {
                    statValue = document.GetValue<object>(statField).ToString();
                }

                entryUI.SetData(rank, stats.playerName, statValue);
                rank++;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(leaderboardContentArea as RectTransform);
    }
}
