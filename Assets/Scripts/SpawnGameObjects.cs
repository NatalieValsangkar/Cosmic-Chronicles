using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Assign your three objects in the inspector
    public ARRaycastManager raycastManager;

    private GameObject currentObject;
    private int currentIndex = 0;

    private void Start()
    {
        if (objectsToSpawn.Length == 0)
        {
            Debug.LogError("No objects assigned to spawn.");
            return;
        }

        SpawnNextObject();
    }

    private void Update()
    {
        if (currentObject == null && currentIndex < objectsToSpawn.Length)
        {
            SpawnNextObject();
        }

        // Detect touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            // Raycast against the current spawned object
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the raycast hit the spawned object
                if (hit.collider.gameObject == currentObject)
                {
                    RedirectToGameScene();
                    SceneManager.LoadScene("2");
                }
            }
        }
    }

    private void SpawnNextObject()
    {
        if (currentIndex >= objectsToSpawn.Length)
            return;

        // Perform an AR raycast to place the object
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinBounds))
        {
            Pose hitPose = hits[0].pose;

            // Adjust position to spawn slightly above the plane
            Vector3 adjustedPosition = hitPose.position + new Vector3(0, 0.1f, 0); // 0.1f is the offset above the plane

            // Instantiate the object and set it as the current object
            currentObject = Instantiate(objectsToSpawn[currentIndex], adjustedPosition, hitPose.rotation);
            currentIndex++;

            // Ensure the object has a collider (e.g., BoxCollider or MeshCollider)
            if (currentObject.GetComponent<Collider>() == null)
            {
                currentObject.AddComponent<BoxCollider>(); // You can use other colliders as needed
            }
        }
    }

    private void RedirectToGameScene()
    {
        // Transition to the correct scene based on the current index
        if (currentIndex == 1)
        {
            SceneManager.LoadScene("2. Saturn Game"); // Make sure the scene name matches
        }
        else if (currentIndex == 2)
        {
            SceneManager.LoadScene("Game2Scene"); // Make sure the scene name matches
        }
        else if (currentIndex == 3)
        {
            SceneManager.LoadScene("Game3Scene"); // Make sure the scene name matches
        }
    }
}
