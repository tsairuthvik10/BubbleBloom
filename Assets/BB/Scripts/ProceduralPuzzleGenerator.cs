using UnityEngine;

public class ProceduralPuzzleGenerator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject bubblePrefab;
    public int numberOfBubblesToPlace = 100;
    public GameObject testfloor;

    private void OnEnable()
    {
        testfloor.SetActive(false);
    }

    public void OnDisable()
    {
        testfloor.SetActive(true);
    }
    void Start()
    {
        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>())
        {
            if (mf.gameObject.GetComponent<MeshCollider>() == null)
            {
                mf.gameObject.AddComponent<MeshCollider>();
            }
        }
        GeneratePuzzle();
    }

    public void GeneratePuzzle()
    {
        MeshCollider[] meshColliders = GetComponentsInChildren<MeshCollider>();
        if (meshColliders.Length == 0)
        {
            Debug.LogError("ProceduralPuzzleGenerator Error: No MeshColliders found.");
            return;
        }

        int bubblesPlaced = 0;
        int attempts = 0;
        while (bubblesPlaced < numberOfBubblesToPlace && attempts < numberOfBubblesToPlace * 20)
        {
            attempts++;
            MeshCollider randomCollider = meshColliders[Random.Range(0, meshColliders.Length)];
            Bounds bounds = randomCollider.bounds;
            Vector3 raycastStartPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + 1f,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            RaycastHit hit;
            if (Physics.Raycast(raycastStartPoint, Vector3.down, out hit, bounds.size.y + 2f) && hit.collider == randomCollider)
            {
                if (Vector3.Dot(hit.normal, Vector3.up) > -0.3f)
                {
                    Instantiate(bubblePrefab, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(hit.normal));
                    bubblesPlaced++;
                }
            }
        }
    }
}