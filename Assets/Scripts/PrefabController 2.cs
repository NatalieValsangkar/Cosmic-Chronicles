// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PrefabController : MonoBehaviour
// {
//     public List<GameObject> prefabsToMove; // Assign your 5 prefabs in the Inspector
//     public List<Transform> startPoints; // Assign 5 empty GameObjects as start points in the Inspector
//     public Transform centerPoint; // Assign an empty GameObject as the center point in the Inspector
//     public GameObject vfxPrefab; // Assign your VFX/Particle System prefab in the Inspector
//     public GameObject finalPrefab; // Assign your final prefab in the Inspector
//     public float moveDuration = 2f; // Duration for prefabs to move to the center
//     public float vfxDuration = 2f; // Duration of the VFX
//     public float finalPrefabDelay = 2f; // Delay before spawning the final prefab

//     private void Start()
//     {
//         // Start the animation sequence
//         // StartCoroutine(AnimationSequence());
//     }

//     public void StartAnimation()
//     {
//         StartCoroutine(AnimationSequence());
//     }

//     public IEnumerator AnimationSequence()
//     {
//         // 1. Move prefabs to the center
//         yield return StartCoroutine(MovePrefabsToCenter());

//         // 2. Instantiate and play VFX
//         GameObject vfxInstance = Instantiate(vfxPrefab, centerPoint.position, centerPoint.rotation);
//         yield return new WaitForSeconds(vfxDuration);

//         // 3. Destroy the 5 prefabs
//         foreach (GameObject prefab in prefabsToMove)
//         {
//             Destroy(prefab);
//         }

//         // 4. Destroy the VFX
//         Destroy(vfxInstance);

//         // 5. Wait for the specified delay
//         yield return new WaitForSeconds(finalPrefabDelay);

//         // 6. Spawn the final prefab
//         Instantiate(finalPrefab, centerPoint.position, centerPoint.rotation);
//     }

//     private IEnumerator MovePrefabsToCenter()
//     {
//         float elapsedTime = 0;
//         List<Vector3> initialPositions = new List<Vector3>();
//         List<Quaternion> initialRotations = new List<Quaternion>();

//         // Store initial positions and rotations
//         for (int i = 0; i < prefabsToMove.Count; i++)
//         {
//             initialPositions.Add(prefabsToMove[i].transform.position);
//             initialRotations.Add(prefabsToMove[i].transform.rotation);
//              // Set the position of each prefab to its corresponding start point
//             prefabsToMove[i].transform.position = startPoints[i].position;
//             prefabsToMove[i].transform.rotation = startPoints[i].rotation;

//         }

//         while (elapsedTime < moveDuration)
//         {
//             elapsedTime += Time.deltaTime;
//             float t = Mathf.Clamp01(elapsedTime / moveDuration);

//             for (int i = 0; i < prefabsToMove.Count; i++)
//             {
//                 // Interpolate position
//                 prefabsToMove[i].transform.position = Vector3.Lerp(startPoints[i].position, centerPoint.position, t);

//                 // Interpolate rotation (optional, if you want them to face the center)
//                 prefabsToMove[i].transform.rotation = Quaternion.Slerp(startPoints[i].rotation, centerPoint.rotation, t);
//             }

//             yield return null;
//         }

//         // Ensure prefabs are exactly at the center at the end
//         for (int i = 0; i < prefabsToMove.Count; i++)
//         {
//             prefabsToMove[i].transform.position = centerPoint.position;
//             prefabsToMove[i].transform.rotation = centerPoint.rotation;

//         }
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabController : MonoBehaviour
{
    public GameObject vfxPrefab; // Assign your VFX/Particle System prefab in the Inspector
    public GameObject finalPrefab; // Assign your final prefab in the Inspector
    public float vfxDuration = 2f; // Duration of the VFX
    public float finalPrefabDelay = 2f; // Delay before spawning the final prefab
    public float heightAboveImage = 0.1f; // Height above the tracked image to spawn prefabs

    public ARTrackedImageManager trackedImageManager; // Assign your ARTrackedImageManager in the Inspector

    public void StartAnimation()
    {
        // Find the tracked image that triggered the animation
        // ARTrackedImage trackedImage = FindTrackedImage();

        // if (trackedImage != null)
        // {
            // StartCoroutine(AnimationSequence(trackedImage));
        // }
    }

    public IEnumerator AnimationSequence(ARTrackedImageManager trackedImage)
    {
        // Calculate the position above the tracked image
        Vector3 spawnPosition = trackedImage.transform.position + trackedImage.transform.up * heightAboveImage;

        yield return new WaitForSeconds(5f);
        // 1. Instantiate and play VFX at the calculated position
        GameObject vfxInstance = Instantiate(vfxPrefab, spawnPosition, trackedImage.transform.rotation);
        yield return new WaitForSeconds(vfxDuration);

        // 2. Destroy the VFX
        Destroy(vfxInstance);

        // 3. Wait for the specified delay
        yield return new WaitForSeconds(finalPrefabDelay);

        // 4. Spawn the final prefab at the calculated position
        Instantiate(finalPrefab, spawnPosition, trackedImage.transform.rotation);
    }

    private ARTrackedImage FindTrackedImage()
    {
        foreach (var trackedImage in trackedImageManager.trackables)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {

                return trackedImage;
            }
        }
        return null;
    }
}