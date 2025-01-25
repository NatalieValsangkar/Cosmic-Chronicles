using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARPlacement : MonoBehaviour
{ 
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    public GameObject shoot;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    public ARRaycastManager aRRaycastManager;
    public bool placementPoseIsValid = false;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        shoot.SetActive(false);
    }

    void Update()
    {
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
            shoot.SetActive(true);
        }

        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // Pinch zoom functionality        
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (Input.touchCount == 2)
 {
     // ... (Pinch detection code) ...

     if (spawnedObject != null)
     {
         float pinchAmount = deltaMagnitudeDiff * 0.02f; 
         Vector3 newScale = spawnedObject.transform.localScale + new Vector3(pinchAmount, pinchAmount, pinchAmount);

         // Clamp the scale within limits
         float minScale = 0.5f;
         float maxScale = 3.0f;
         newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
         newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
         newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);
         spawnedObject.transform.localScale = newScale; 
     }
 }
        }
    }

    void UpdatePlacementIndicator()
    {
        if (spawnedObject == null && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    public void ARPlaceObject()
    {
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);
    }
}
