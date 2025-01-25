using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;
    public GameObject[] ArPrefabs; // These are your Canvas UI prefabs
    public GameObject[] Ar3dModels; // Array for 3D models

    List<GameObject> ARObjects = new List<GameObject>();
    private int spawnedUICount = 0; // Counter for spawned UI prefabs (only increments)
    private int spawnedModelCount = 0;
    private int uiSpawnedTracker = 0; // Separate variable to track UI spawning for animation
    private bool animationPlayed = false;

    public PrefabController prefabController; // Assign your PrefabController in the Inspector
    public TextMeshProUGUI scanCountText; // Assign a TextMeshPro UI element in the Inspector
    public Canvas canvasUI; // Assign your Canvas in the Inspector

    void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
    }

    private void Start()
    {
        UpdateScanCountText(); // Initialize the text at the start
    }

    void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    // Event Handler
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Spawn UI Prefabs
            SpawnUIPrefabs(trackedImage);

            // Spawn 3D Models
            Spawn3DModels(trackedImage);
        }

        // Play animation logic (Modified to use uiSpawnedTracker)
        if (uiSpawnedTracker == ArPrefabs.Length && !animationPlayed)
        {
            PlayAnimationAndCleanup();
        }

        //Update tracking position for UI and 3D Models
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackingPosition(trackedImage);
        }
    }

    private void SpawnUIPrefabs(ARTrackedImage trackedImage)
    {
        foreach (var arPrefab in ArPrefabs)
        {
            if (trackedImage.referenceImage.name == arPrefab.name)
            {
                // Check if this UI prefab has already been tracked
                bool uiAlreadyTracked = uiSpawnedTracker >= ArPrefabs.Length;

                if (!uiAlreadyTracked)
                {
                    var newPrefab = Instantiate(arPrefab, canvasUI.transform);
                    newPrefab.name = arPrefab.name + "_UI(Clone)"; // Unique name for UI
                    ARObjects.Add(newPrefab);

                    spawnedUICount++; // Increment UI counter (This will only ever increase)
                    uiSpawnedTracker++;

                    Debug.Log($"Spawned UI {arPrefab.name}, Count: {spawnedUICount}");
                    UpdateScanCountText(); // Update the scan count text

                    // Start the destroy coroutine for the newly spawned UI prefab
                    StartCoroutine(DestroyPrefabAfterDelay(newPrefab, 5f));
                }
            }
        }
    }

    private void Spawn3DModels(ARTrackedImage trackedImage)
    {
        foreach (var arModel in Ar3dModels)
        {
            if (trackedImage.referenceImage.name == arModel.name)
            {
                // Check if this 3D model has already been spawned
                bool modelAlreadySpawned = ARObjects.Exists(obj => obj != null && obj.name == arModel.name + "_3D(Clone)");

                if (!modelAlreadySpawned)
                {
                    var newModel = Instantiate(arModel, trackedImage.transform.position, trackedImage.transform.rotation);
                    newModel.name = arModel.name + "_3D(Clone)"; // Unique name for 3D Model
                    ARObjects.Add(newModel);
                    spawnedModelCount++;

                    // Adjust 3D model position to be slightly above the image
                    newModel.transform.localPosition += new Vector3(0, 0f, 0); // Adjust as needed

                    // Start the destroy coroutine for the newly spawned 3D model prefab
                    StartCoroutine(DestroyPrefabAfterDelay(newModel, 5f));
                }
            }
        }
    }

    private void UpdateTrackingPosition(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            foreach (var gameObject in ARObjects)
            {
                if (gameObject != null)
                {
                    // UI elements
                    if (gameObject.name == trackedImage.referenceImage.name + "_UI(Clone)")
                    {
                        // Nothing specific to do here since UI is parented to the Canvas
                    }
                    // 3D models
                    else if (gameObject.name == trackedImage.referenceImage.name + "_3D(Clone)")
                    {
                        gameObject.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
                        gameObject.transform.localPosition += new Vector3(0, 0.1f, 0);
                    }
                }
            }
        }
    }
    private void PlayAnimationAndCleanup()
    {
        animationPlayed = true;
        Debug.Log("All UI objects spawned. Playing animation.");

        // Play your animation
        if (prefabController != null)
        {
            prefabController.StartCoroutine(prefabController.AnimationSequence(trackedImages)); // Start the animation sequence
        }
    }

    private IEnumerator DestroyPrefabAfterDelay(GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (prefab != null)
        {
            ARObjects.Remove(prefab);
            Destroy(prefab);
        }
    }

    private void UpdateScanCountText()
    {
        if (scanCountText != null)
        {
            scanCountText.text = $"Scanned: {spawnedUICount} / {ArPrefabs.Length}";
        }
    }
}