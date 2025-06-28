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
    public float comboResetTime = 2.5f;
    private float comboTimer;

    [Header("Scoring")]
    public int currentScore = 0;
    public int currentCombo = 0;
    public int combosToCompleteForPlant = 3;
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
            EndLevel(true); // End because of time out
        }

        if (currentCombo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
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
        comboTimer = comboResetTime;

        currentCombo++;
        sessionLongestCombo = Mathf.Max(sessionLongestCombo, currentCombo);
        int pointsToAdd = 10 + (currentCombo > 1 ? Mathf.RoundToInt(10 * (comboMultiplier * (currentCombo - 1))) : 0);
        currentScore += pointsToAdd;

        if (UIManager.Instance != null) UIManager.Instance.UpdateScore(currentScore);
        if (UIManager.Instance != null) UIManager.Instance.UpdateCombo(currentCombo);
        if (FeedbackManager.Instance != null) FeedbackManager.Instance.TriggerHapticFeedback();
    }

    public void EndLevel(bool timedOut = false)
    {
        if (levelEnded) return;
        levelEnded = true;

        if (!timedOut)
        {
            if (FeedbackManager.Instance != null) FeedbackManager.Instance.ShowToast("No more moves!");
        }

        Debug.Log("LEVEL ENDED! Final Score: " + currentScore);

        if (plantPrefab != null && sessionLongestCombo > combosToCompleteForPlant)
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

        // After a combo resets, check if any moves are left
        CheckForPossibleMoves();
    }

    // ** NEW FUNCTION TO DETECT STALEMATE **
    private void CheckForPossibleMoves()
    {
        if (levelEnded) return;

        GameObject[] remainingBubbles = GameObject.FindGameObjectsWithTag("Bubble");

        // If no bubbles are left, the player won!
        if (remainingBubbles.Length == 0)
        {
            EndLevel();
            return;
        }

        // Check each remaining bubble to see if it can start a chain
        foreach (var bubbleGO in remainingBubbles)
        {
            Bubble bubble = bubbleGO.GetComponent<Bubble>();
            if (bubble != null)
            {
                // Check for other bubbles within its chain radius
                Collider[] neighbors = Physics.OverlapSphere(bubble.transform.position, bubble.chainRadius);
                if (neighbors.Length > 1) // More than just itself
                {
                    // Found a possible move, so we can exit early.
                    return;
                }
            }
        }

        // If the loop finishes, it means no bubble has any neighbors. The game is stuck.
        Debug.Log("Stalemate detected! Ending level.");
        EndLevel();
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
