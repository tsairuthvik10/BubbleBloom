using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Assets")]
    public GameObject plantPrefab;

    [Header("Game Loop Settings")]
    public float startingTime = 30.0f;
    public float timeAddedPerPop = 0.75f;
    private float currentTime;
    public float comboResetTime = 2.5f;
    private float comboTimer;

    [Header("Scoring")]
    public int currentScore = 0;
    public int currentCombo = 0;
    public float comboMultiplier = 1.5f;

    private int sessionLongestCombo = 0;
    private bool levelEnded = false;
    private Vector3 lastPoppedPosition;
    private bool isGameActive = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        isGameActive = false;
    }

    void Update()
    {
        if (!isGameActive || levelEnded) return;

        currentTime -= Time.deltaTime;
        if (UIManager.Instance != null) UIManager.Instance.UpdateTimer(currentTime);
        if (currentTime <= 0) EndLevel();

        if (currentCombo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0) ResetCombo();
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        levelEnded = false;
        currentTime = startingTime;
        currentScore = 0;
        sessionLongestCombo = 0;
        ResetCombo();
        Debug.Log("Game timer has been started!");
    }

    public int OnBubblePopped(Vector3 popPosition)
    {
        if (levelEnded || !isGameActive) return 0;
        if (currentCombo == 0) comboTimer = comboResetTime;

        lastPoppedPosition = popPosition;
        currentTime += timeAddedPerPop;

        currentCombo++;
        sessionLongestCombo = Mathf.Max(sessionLongestCombo, currentCombo);
        int pointsToAdd = 10 + (currentCombo > 1 ? Mathf.RoundToInt(10 * (comboMultiplier * (currentCombo - 1))) : 0);
        currentScore += pointsToAdd;

        if (UIManager.Instance != null) UIManager.Instance.UpdateScore(currentScore);
        if (UIManager.Instance != null) UIManager.Instance.UpdateCombo(currentCombo);

        return pointsToAdd;
    }

    public void ResetCombo()
    {
        if (currentCombo > 3)
        {
            SpawnPlantAt(lastPoppedPosition);
        }
        currentCombo = 0;
        if (UIManager.Instance != null) UIManager.Instance.UpdateCombo(currentCombo);
    }

    // ** NEW FUNCTION TO SPAWN PLANT ON THE GROUND **
    private void SpawnPlantAt(Vector3 originPosition)
    {
        if (plantPrefab == null) return;

        Vector3 spawnPosition = originPosition;

        // Raycast downwards from the last bubble's position to find the ground
        RaycastHit hit;
        if (Physics.Raycast(originPosition, Vector3.down, out hit, 50f))
        {
            // Found a surface, spawn the plant on that point
            spawnPosition = hit.point;
        }
        // If the raycast fails, it will just spawn where the last bubble was (safe fallback)

        Instantiate(plantPrefab, spawnPosition, Quaternion.identity);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayPlantGrow();
        if (VFXManager.Instance != null) VFXManager.Instance.PlayPlantGrowVFX(spawnPosition);
    }

    public void EndLevel()
    {
        if (levelEnded) return;
        levelEnded = true;
        ResetCombo();

        if (LeaderboardManager.Instance != null) LeaderboardManager.Instance.SubmitScore(currentScore, sessionLongestCombo);
        if (UIManager.Instance != null) UIManager.Instance.ShowSummaryPanel();
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
