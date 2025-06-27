using UnityEngine;

public class PortalEditorTest : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject portalPrefab;
    public GameObject environmentToEnable;
    public Camera mainCamera;
    public float moveSpeed = 5f;

    private GameObject spawnedPortal;

    void Awake()
    {
        if (environmentToEnable != null)
        {
            environmentToEnable.SetActive(false);
        }
    }

    void Update()
    {
        // --- 1. Handle Input ---
        if (Input.GetMouseButtonDown(0))
        {
            // If the virtual world is active, we are trying to pop bubbles
            if (environmentToEnable != null && environmentToEnable.activeSelf)
            {
                PopBubble();
            }
            // Otherwise, we are trying to place the portal
            else if (spawnedPortal == null)
            {
                PlacePortal();
            }
        }

        // --- 2. Simulate Walking Forward with 'W' Key ---
        if (spawnedPortal != null)
        {
            if (Input.GetKey(KeyCode.W))
            {
                mainCamera.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }

            // Check if we've "walked" through the portal
            CheckForTeleport();
        }
    }

    void PlacePortal()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            spawnedPortal = Instantiate(portalPrefab, hit.point, Quaternion.identity);
            Vector3 lookAtPos = new Vector3(mainCamera.transform.position.x, spawnedPortal.transform.position.y, mainCamera.transform.position.z);
            spawnedPortal.transform.LookAt(lookAtPos);
            Debug.Log("Portal spawned. Press and hold 'W' to walk forward.");
        }
    }

    void PopBubble()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Bubble"))
            {
                hit.collider.GetComponent<Bubble>().Pop();
            }
        }
    }

    void CheckForTeleport()
    {
        if (environmentToEnable.activeSelf) return;

        float distanceToPortal = Vector3.Distance(mainCamera.transform.position, spawnedPortal.transform.position);
        Vector3 directionToCamera = (mainCamera.transform.position - spawnedPortal.transform.position).normalized;
        float dotProduct = Vector3.Dot(spawnedPortal.transform.forward, directionToCamera);

        if (distanceToPortal < 1.5f && dotProduct < 0)
        {
            EnablePortalWorld();
        }
    }

    void EnablePortalWorld()
    {
        mainCamera.clearFlags = CameraClearFlags.Skybox;
        if (environmentToEnable != null)
        {
            environmentToEnable.SetActive(true);
        }
        if (spawnedPortal != null)
        {
            spawnedPortal.SetActive(false);
        }
        Debug.Log("Teleport successful! Virtual World is now active.");
    }
}