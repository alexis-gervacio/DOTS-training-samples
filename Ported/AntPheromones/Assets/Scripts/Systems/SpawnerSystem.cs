﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;


public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in AntSpawnerEntity spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.AntCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.AntPrefab);
                    var translation = new Translation { Value = new float3(0, 0, 0) };
                    ecb.SetComponent(instance, translation);
                }
            }).WithoutBurst().Run();        // why is WithoutBurst needed but wasn't for the samples?

        ecb.Playback(EntityManager);
    }
}
