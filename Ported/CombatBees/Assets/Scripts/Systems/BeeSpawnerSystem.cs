﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BeeSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var field = GetSingleton<FieldAuthoring>();
        var beeParams = GetSingleton<BeeControlParams>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var random = new Unity.Mathematics.Random(1234);

        Entities
            .WithName("Bee_Spawner")
            //.ForEach((Entity spawnerEntity, ref BeeSpawner spawner, in Translation spawnerPos) =>
            .ForEach((Entity spawnerEntity, ref BeeSpawner spawner) =>
            {
                Debug.Log("beespawner working, cout = !!" + spawner.count);
                for (int i = 0; i < spawner.count; i++)
                {
                    var bee = ecb.Instantiate(spawner.beePrefab);

                    int team = (int)spawner.team;
                    float3 pos = math.right() * (-field.size.x * .4f + field.size.x * .8f * team);
                    ecb.SetComponent(bee, new Translation { Value = pos });

                    /*
                    float size = random.NextFloat(beeParams.minBeeSize, beeParams.minBeeSize);
                    ecb.SetComponent(bee, new Size { value = size });
                    ecb.SetComponent(bee, new NonUniformScale { Value = new float3(size, size, size) });
                    ecb.SetComponent(bee, new Velocity { vel = random.NextFloat3() * spawner.maxSpawnSpeed });
                    */

                    float size = random.NextFloat(beeParams.minBeeSize, beeParams.maxBeeSize);
                    ecb.AddComponent(bee, new Size { value = size });
                    //ecb.AddComponent(bee, new NonUniformScale { Value = new float3(size, size, size) });
                    ecb.AddComponent(bee, new Velocity { vel = random.NextFloat3() * spawner.maxSpawnSpeed });
                }

                ecb.DestroyEntity(spawnerEntity);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}