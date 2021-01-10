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
}
