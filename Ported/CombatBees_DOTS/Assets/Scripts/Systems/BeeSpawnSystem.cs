using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class BeeSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .ForEach((Entity spawnerEntity, in BeeSpawner spawner) =>
        {
            NativeArray<Entity> bees = CollectionHelper.CreateNativeArray<Entity>(spawner.BeeCount, Allocator.Temp);
            ecb.Instantiate(spawner.BeePrefab, bees);

            foreach (var bee in bees)
            {
                ecb.SetComponent(bee, new Translation() { Value = spawner.SpawnPoint });
                
            }
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        // disable after first update 
        this.Enabled = false;
    }
}