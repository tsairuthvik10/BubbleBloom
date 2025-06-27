using UnityEngine;
using UnityEngine.UI; // **NEW**: Required for LayoutRebuilder
using TMPro;
using Firebase.Firestore;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject hudPanel;
    public GameObject summaryPanel;
    public GameObject leaderboardPanel;
    public GameObject profileCreationPanel;

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

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (leaderboardSortDropdown != null)
        {
            leaderboardSortDropdown.onValueChanged.AddListener(delegate { OnSortChanged(); });
        }
        
        if(summaryPanel != null) summaryPanel.SetActive(false);
        if(leaderboardPanel != null) leaderboardPanel.SetActive(false);
        if(hudPanel != null && ProfileManager.Instance != null && !string.IsNullOrEmpty(ProfileManager.Instance.PlayerName)) 
        {
            hudPanel.SetActive(true);
        }
    }
    
    public void UpdateScore(int score) => scoreText.text = $"Score: {score}";
    public void UpdateCombo(int combo)
    {
        if(comboText == null) return;
        comboText.gameObject.SetActive(combo > 1);
        if(combo > 1) comboText.text = $"Combo x{combo}!";
    }
    
    public void UpdateTimer(float time)
    {
        if (timerText != null)
        {
            timerText.text = time > 0 ? $"Time: {Mathf.CeilToInt(time)}" : "Time: 0";
        }
    }

    public void ShowSummaryPanel()
    {
        if(summaryPanel == null) return;
        //Debug.Log("LEVEL ENDED! UIM: ShowSummaryPanel: Final Score: " + GameManager.Instance.currentScore);
        int finalScore = GameManager.Instance.currentScore;
        int longestCombo = GameManager.Instance.GetSessionLongestCombo();
        //Debug.Log("LEVEL ENDED! UIM: ShowSummaryPanel: Final Score after getting longest combo: " + GameManager.Instance.currentScore);
        if (summaryScoreText != null) summaryScoreText.text = $"Final Score: {finalScore}";
        if(summaryComboText != null) summaryComboText.text = $"Best Combo: {longestCombo}";

        if(hudPanel != null) hudPanel.SetActive(false);
        if(leaderboardPanel != null) leaderboardPanel.SetActive(false);
        summaryPanel.SetActive(true);
    }

    public void OnViewLeaderboardClicked()
    {
        if(leaderboardPanel == null || LeaderboardManager.Instance == null) return;
        if(summaryPanel != null) summaryPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        OnSortChanged();
    }

    public void OnCloseLeaderboardClicked()
    {
        if(leaderboardPanel == null || summaryPanel == null) return;
        leaderboardPanel.SetActive(false);
        summaryPanel.SetActive(true);
    }

    private async void OnSortChanged()
    {
        if(!leaderboardPanel.activeSelf || LeaderboardManager.Instance == null) return;

        string sortByField = "highestScore";
        if(leaderboardSortDropdown != null)
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
        // Clear old entries before displaying new ones
        foreach (Transform child in leaderboardContentArea) Destroy(child.gameObject);

        if (snapshot == null) 
        {
            Debug.Log("Leaderboard snapshot is null! Check Firebase connection and rules.");
            return;
        }

        int rank = 1;
        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            if (document.Exists)
            {
                GameObject entryGO = Instantiate(leaderboardEntryPrefab, leaderboardContentArea);
                LeaderboardEntryUI entryUI = entryGO.GetComponent<LeaderboardEntryUI>();

                if (entryUI == null)
                {
                    Debug.Log("FATAL: LeaderboardEntry_Prefab is MISSING the LeaderboardEntryUI script! Please attach it.");
                    continue;
                }

                PlayerStats stats = document.ConvertTo<PlayerStats>();
                
                string statValue = "N/A";
                if(document.ContainsField(statField))
                {
                    statValue = document.GetValue<object>(statField).ToString();
                }

                // Use the script to set the data safely
                entryUI.SetData(rank, stats.playerName, statValue);
                rank++;
            }
        }
        
        // ** THE FIX **
        // Force the layout group to update immediately. This ensures the newly
        // instantiated items are properly sized and positioned in the same frame.
        LayoutRebuilder.ForceRebuildLayoutImmediate(leaderboardContentArea as RectTransform);
    }
}