using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct BeeSpawner : IComponentData
{
    public Entity BeePrefab;
    public int BeeCount;
    // public Entity SpawnPoint;
    /// <summary>
    ///  Home hive, also return food posn
    /// todo comment
    /// </summary>
    public Vector3 SpawnPoint;
}