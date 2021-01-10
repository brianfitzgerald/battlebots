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
        ghostMaterial = Resources.Load<Material>("Ghost");
        partMaterial = Resources.Load<Material>("Part");
    }
    public void SetGhost(bool isGhost)
    {
        this.isGhost = isGhost;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.sharedMaterials[0] = isGhost ? ghostMaterial : partMaterial;
        }
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = !isGhost;
        }
    }
}
