using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial class FoodBehaviourSystem : SystemBase
{
    private static float3 carryOffset = new float3(0, -1f, 0);
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithAll<FoodSource>()
            .WithNone<Dead>()
            .ForEach((Entity entity, TransformAspect transform, in HolderBee holder) =>
            {
                bool isHolderBeeDead = HasComponent<Dead>(holder.Holder);
                if (!isHolderBeeDead)
                {
                    var beePosition = GetComponent<Translation>(holder.Holder).Value;
                    transform.Position = beePosition + carryOffset;
                }
                else
                {
                    //TODO ADD gravity if bee died?
                    // Remove holder bee component if bee has died
                    ecb.RemoveComponent(entity, typeof(HolderBee));
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}