using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlaceablePart
{
    public GameObject prefab;
    public string name;
}

public class UserController : MonoBehaviour
{
    private Camera placingCamera;
    private bool placingPart;

    public List<PlaceablePart> placeableParts = new List<PlaceablePart>();

    public GameObject placingPartGhost;

    public PlaceablePart partBeingPlaced;

    public void Start()
    {
        placingCamera = GetComponent<Camera>();
    }
    public void Update()
    {
        if (placingPart)
        {
            RaycastHit hit;
            Ray ray = placingCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                var r = objectHit.transform.GetComponentInParent<RobotController>();
                if (r != null)
                {
                    if (placingPartGhost == null)
                    {
                        placingPartGhost = Instantiate(partBeingPlaced.prefab);
                        placingPartGhost.GetComponent<PartController>().SetGhost(true);
                    }
                    // get the normal of the plane we collided with and use that to orient the part
                    placingPartGhost.transform.rotation = Quaternion.Euler(hit.normal);
                    placingPartGhost.transform.position = hit.point;
                }

            }
            if (Input.GetMouseButtonDown(0))
            {
                placingPartGhost.GetComponent<PartController>().SetGhost(false);
                // TODO add hinge
                placingPart = false;
                placingPartGhost = null;
                partBeingPlaced = new PlaceablePart();
            }
        }

    }
    // TODO: replace with dynamically adding prefabs
    public void PlaceBonk()
    {
        partBeingPlaced = placeableParts[0];
        placingPart = true;
        placingPartGhost = null;
    }
}
