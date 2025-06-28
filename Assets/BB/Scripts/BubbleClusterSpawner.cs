using UnityEngine;
using System.Collections;

public class BubbleClusterSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject bubblePrefab;
    public int bubblesInCluster = 20;
    public float spawnRadius = 4.0f;

    IEnumerator Start()
    {
        if (bubblePrefab == null) yield break;

        yield return new WaitForEndOfFrame();

        int bubblesPlaced = 0;
        int attempts = 0;
        while (bubblesPlaced < bubblesInCluster && attempts < bubblesInCluster * 5)
        {
            attempts++;
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnCheckPoint = transform.position + randomOffset;

            if (Physics.Raycast(spawnCheckPoint + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
            {
                Instantiate(bubblePrefab, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(hit.normal));
                bubblesPlaced++;
            }
        }

        // This object's job is done after spawning.
        gameObject.SetActive(false);
    }
}
