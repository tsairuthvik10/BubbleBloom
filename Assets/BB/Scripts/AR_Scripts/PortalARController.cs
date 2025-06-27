using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARRaycastManager))]
public class PortalARController : MonoBehaviour
{
    [Header("Portal Objects")]
    public GameObject portalPrefab;
    public GameObject environmentToEnable;
    public Camera arCamera;

    private ARRaycastManager raycastManager;
    private GameObject spawnedPortal;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        if (environmentToEnable != null) environmentToEnable.SetActive(false);
    }

    void Update()
    {
        if (spawnedPortal == null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                spawnedPortal = Instantiate(portalPrefab, hitPose.position, Quaternion.identity);
                Vector3 cameraPosition = arCamera.transform.position;
                Vector3 portalPosition = spawnedPortal.transform.position;
                Vector3 lookAtPosition = new Vector3(cameraPosition.x, portalPosition.y, cameraPosition.z);
                spawnedPortal.transform.LookAt(lookAtPosition);
            }
        }

        if (spawnedPortal != null && !environmentToEnable.activeSelf)
        {
            float distanceToPortal = Vector3.Distance(arCamera.transform.position, spawnedPortal.transform.position);
            Vector3 directionToCamera = (arCamera.transform.position - spawnedPortal.transform.position).normalized;
            float dotProduct = Vector3.Dot(spawnedPortal.transform.forward, directionToCamera);

            if (distanceToPortal < 1.5f && dotProduct < 0)
            {
                EnablePortalWorld();
            }
        }
    }

    void EnablePortalWorld()
    {
        var planeManager = GetComponent<ARPlaneManager>();
        if (planeManager != null) planeManager.enabled = false;
        foreach (var plane in FindObjectsOfType<ARPlane>())
        {
            plane.gameObject.SetActive(false);
        }

        arCamera.clearFlags = CameraClearFlags.Skybox;
        if (environmentToEnable != null)
        {
            environmentToEnable.SetActive(true);
        }
        if (spawnedPortal != null)
        {
            spawnedPortal.SetActive(false);
        }
    }
}
