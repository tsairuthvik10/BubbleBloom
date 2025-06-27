using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Assets")]
    public GameObject plantPrefab;

    [Header("Game Loop Settings")]
    public float startingTime = 20.0f;
    public float timeAddedPerPop = 0.5f;
    private float currentTime;

    [Header("Scoring")]
    public int currentScore = 0;
    public int currentCombo = 0;
    public float comboMultiplier = 1.5f;

    private int sessionLongestCombo = 0;
    private bool levelEnded = false;
    private Vector3 lastPoppedPosition;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartLevel();
    }

    void Update()
    {
        if (levelEnded) return;

        currentTime -= Time.deltaTime;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTimer(currentTime);
        }

        if (currentTime <= 0)
        {
            EndLevel();
        }
    }

    void StartLevel()
    {
        levelEnded = false;
        currentTime = startingTime;
        currentScore = 0;
        sessionLongestCombo = 0;
        ResetCombo();
    }

    public void OnBubblePopped(Vector3 popPosition)
    {
        if (levelEnded) return;

        lastPoppedPosition = popPosition;
        currentTime += timeAddedPerPop;

        currentCombo++;
        sessionLongestCombo = Mathf.Max(sessionLongestCombo, currentCombo);
        int pointsToAdd = 10 + (currentCombo > 1 ? Mathf.RoundToInt(10 * (comboMultiplier * (currentCombo - 1))) : 0);
        currentScore += pointsToAdd;

        if (UIManager.Instance != null) UIManager.Instance.UpdateScore(currentScore);
        if (UIManager.Instance != null) UIManager.Instance.UpdateCombo(currentCombo);
        if (FeedbackManager.Instance != null) FeedbackManager.Instance.TriggerHapticFeedback();
    }

    public void EndLevel()
    {
        if (levelEnded) return;
        levelEnded = true;
        Debug.Log("LEVEL ENDED! Final Score: " + currentScore);

        if (plantPrefab != null && sessionLongestCombo > 5)
        {
            Instantiate(plantPrefab, lastPoppedPosition, Quaternion.identity);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayPlantGrow();
            if (VFXManager.Instance != null) VFXManager.Instance.PlayPlantGrowVFX(lastPoppedPosition);
        }

        if (LeaderboardManager.Instance != null) LeaderboardManager.Instance.SubmitScore(currentScore, sessionLongestCombo);
        if (UIManager.Instance != null) UIManager.Instance.ShowSummaryPanel();
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        if (UIManager.Instance != null) UIManager.Instance.UpdateCombo(currentCombo);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetSessionLongestCombo()
    {
        return sessionLongestCombo;
    }
}