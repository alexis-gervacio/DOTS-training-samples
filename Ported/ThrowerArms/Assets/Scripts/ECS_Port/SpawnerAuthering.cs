﻿using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

// ReSharper disable once InconsistentNaming
public struct SpawnerComponent : IComponentData
{
    public float3 center;
    public float3 extend;
    public Entity spawnEntity;
    public float frequency;
    public float timeToNextSpawn;
    public Random random;
    public float2 scaleRange;
    public float3 velocity;
}

public class SpawnerAuthering : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject spawnPrefab;
    public Vector2 scaleRange;
    public Vector3 velocity;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnEntity = conversionSystem.GetPrimaryEntity(spawnPrefab);
        dstManager.AddComponentData(entity, new SpawnerComponent() {
            center = transform.position,
            extend = transform.localScale,
            frequency = 50f,
            spawnEntity = spawnEntity,
            timeToNextSpawn = 0,
            random = new Random(783465),
            scaleRange = scaleRange,
            velocity = velocity
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(spawnPrefab);
    }
}