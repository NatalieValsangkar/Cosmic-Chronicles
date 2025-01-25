using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class Spawner : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public Material greenMaterial;

    public GameObject spacePrefab;
    private GameObject spaceItem = null;

    private List<ARPlane> greenPlanes = new List<ARPlane>();

    void OnEnable()
    {
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (spaceItem != null)
        {
            return;
        }
        foreach (var plane in args.added)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                plane.GetComponent<MeshRenderer>().material = greenMaterial;

                //spaceItem = Instantiate(spacePrefab, plane.transform.position, plane.transform.rotation);
                //spaceItem.transform.SetParent(plane.transform);

            }
        }
    }
}
