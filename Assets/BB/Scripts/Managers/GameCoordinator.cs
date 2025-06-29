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

    [Header("On-Site AR Setup (AR Mode Only)")]
    [Tooltip("This must match the number of anchors you placed in the Augg.io CMS.")]
    public int expectedAnchorCountInAR = 5;

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

        // 2. Calculate how many bubbles to wait for based on the mock setup.
        int expectedBubbles = mockAnchorTransforms.Count * bubbleClusterSpawnerPrefab.GetComponent<BubbleClusterSpawner>().bubblesInCluster;
        yield return StartCoroutine(WaitForSpawningToComplete(expectedBubbles));
    }

    // This runs in the final AR build.
    private IEnumerator WaitForRealSpawnersAndStartGame()
    {
        Debug.Log("Coordinator: Running in AR Mode, waiting for Augg.io spawners.");
        // Give the Augg.io system time to localize and instantiate its objects.
        yield return new WaitForSeconds(5);

        // Calculate how many bubbles to wait for based on the AR setup.
        int expectedBubbles = expectedAnchorCountInAR * bubbleClusterSpawnerPrefab.GetComponent<BubbleClusterSpawner>().bubblesInCluster;
        yield return StartCoroutine(WaitForSpawningToComplete(expectedBubbles));
    }

    /// <summary>
    /// This coroutine reliably waits until all bubbles have finished spawning before starting the game.
    /// It works by checking if the bubble count is stable.
    /// </summary>
    private IEnumerator WaitForSpawningToComplete(int totalBubblesToWaitFor)
    {
        Debug.Log($"Coordinator: Waiting for {totalBubblesToWaitFor} bubbles to spawn...");

        yield return new WaitForSeconds(2.0f); // Initial delay

        int lastBubbleCount = -1;
        int currentBubbleCount = 0;
        int stableChecks = 0;
        int maxWaitFrames = 300; // Failsafe (approx 5 seconds)
        int frameCount = 0;

        while (stableChecks < 4 && frameCount < maxWaitFrames)
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
            frameCount++;
            yield return null; // Wait for the next frame
        }

        Debug.Log($"Spawning complete! Found {currentBubbleCount} bubbles.");

        // Now that spawning is confirmed complete, start the game.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}