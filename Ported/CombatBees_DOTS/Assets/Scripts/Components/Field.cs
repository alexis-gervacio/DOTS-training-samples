using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct Field : IComponentData
{
    [Tooltip("Hard bounds for entire field, including faction zones and free zones")]
    public float3 HardBounds;
    [Tooltip("Free zone bounds (not home to any faction)")]
    public float3 FreeZoneBounds;
}