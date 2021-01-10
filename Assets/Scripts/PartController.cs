using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    COSMETIC,
    HAMMER,
    WHEEL
}
public class PartController : MonoBehaviour
{
    public Transform anchor;
    public PartType partType;
    private bool isGhost = false;

    private Material ghostMaterial;
    private Material partMaterial;
    public void Start()
    {
        ghostMaterial = Resources.Load("Material/Ghost.mat", typeof(Material)) as Material;
        partMaterial = Resources.Load("Material/Part.mat", typeof(Material)) as Material;
    }
    public void SetGhost(bool isGhost)
    {
        this.isGhost = isGhost;
        GetComponent<MeshRenderer>().material = isGhost ? ghostMaterial : partMaterial;
    }
}
