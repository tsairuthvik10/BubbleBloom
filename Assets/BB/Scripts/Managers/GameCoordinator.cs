using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCoordinator : MonoBehaviour
{
    public static GameCoordinator Instance { get; private set; }

    [Header("Mode Configuration")]
    [Tooltip("Check this ON for the editor test scene, and OFF for the final AR build.")]
    public bool isEditorTestMode = true;

    [Header("Editor Test Setup (Test Mode Only)")]
    public GameObject bubbleClusterSpawnerPrefab;
    public Transform environmentRoot; // The root of your placeholder scan
    public List<Transform> mockAnchorTransforms;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // This single script now handles both scenarios.
        if (isEditorTestMode)
        {
            StartCoroutine(SimulateAndStartGame());
        }
        else
        {
            // In AR mode, we simply wait for the Augg.io system to create the spawners.
            StartCoroutine(WaitForRealSpawnersAndStartGame());
        }
    }

    // This runs only in the editor test scene.
    private IEnumerator SimulateAndStartGame()
    {
        Debug.Log("Coordinator: Running in Editor Test Mode.");
        if (bubbleClusterSpawnerPrefab == null)
        {
            Debug.LogError("Bubble Cluster Spawner Prefab is not assigned in GameCoordinator!");
            yield break;
        }

        // 1. Create the spawner objects at the mock anchor positions.
        foreach (Transform anchorTransform in mockAnchorTransforms)
        {
            Instantiate(bubbleClusterSpawnerPrefab, anchorTransform.position, anchorTransform.rotation, environmentRoot);
        }

        // 2. Wait for those spawners to finish creating bubbles.
        yield return StartCoroutine(WaitForSpawningToComplete());
    }

    // This runs in the final AR build.
    private IEnumerator WaitForRealSpawnersAndStartGame()
    {
        Debug.Log("Coordinator: Running in AR Mode, waiting for Augg.io spawners.");
        // Give the Augg.io system time to localize and instantiate its objects.
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(WaitForSpawningToComplete());
    }

    /// <summary>
    /// This coroutine reliably waits until all bubbles have finished spawning before starting the game.
    /// It works by checking if the bubble count is stable.
    /// </summary>
    private IEnumerator WaitForSpawningToComplete()
    {
        Debug.Log("Coordinator: Waiting for bubbles to spawn...");

        // Give the spawners a moment to run their Start() methods.
        yield return new WaitForSeconds(1.0f);

        int lastBubbleCount = -1;
        int currentBubbleCount = 0;
        int stableChecks = 0;
        int maxChecks = 10; // Failsafe to prevent getting stuck forever
        int checksDone = 0;

        // We check for a stable count to be sure all spawners have finished.
        while (stableChecks < 3 && checksDone < maxChecks)
        {
            currentBubbleCount = GameObject.FindGameObjectsWithTag("Bubble").Length;

            if (currentBubbleCount == lastBubbleCount && currentBubbleCount > 0)
            {
                stableChecks++;
            }
            else
            {
                stableChecks = 0; // Reset if the count changes
            }

            lastBubbleCount = currentBubbleCount;
            checksDone++;
            yield return new WaitForSeconds(0.5f); // Wait between checks
        }

        Debug.Log($"Spawning complete! Found {currentBubbleCount} bubbles.");

        // Now that spawning is confirmed complete, start the game.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}
