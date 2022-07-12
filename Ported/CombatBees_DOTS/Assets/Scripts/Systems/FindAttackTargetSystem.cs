using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class FindAttackTargetSystem : SystemBase
{
    // Queries should not be created on the spot in OnUpdate, so they are cached in fields.
    // EntityQuery yellowJacketQuery;
    // NativeArray<Translation> yellowJacketTranslations;
    // private NativeArray<Entity> yellowJacketEntities;
    
    protected override void OnUpdate()
    {
        EntityQuery yellowJacketQuery = GetEntityQuery(typeof(Bee), typeof(FactionYellowJacket), typeof(Translation));
        // yellowJacketQuery = GetEntityQuery(typeof(Bee), typeof(FactionYellowJacket), typeof(Translation));
        NativeArray<Translation> yellowJacketTranslations = yellowJacketQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<Entity> yellowJacketEntities = yellowJacketQuery.ToEntityArray(Allocator.Temp);
        // TODO are the above even nevessary
        
        // yellowJacketTranslations = yellowJacketQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        // yellowJacketEntities = yellowJacketQuery.ToEntityArray(Allocator.Temp);
        
        // Entities
        //     .WithReadOnly(yellowJacketTranslations) // CONVERT TO WITH READONLY, ARE THERE ACTUALLY SAFE CHEKS IN COMPILER?
        //     .WithReadOnly(yellowJacketEntities)
        //     .WithDisposeOnCompletion(yellowJacketTranslations) // CONVERT TO WITH READONLY, ARE THERE ACTUALLY SAFE CHEcKS IN COMPILER?
        //     .WithDisposeOnCompletion(yellowJacketEntities)
        //     .WithAll<FactionHoneyBee>()
        //     .ForEach((ref Translation Translation, ref Bee bee) =>
        //     {
        //         // Return if bee state is not idle or attacking
        //         // TODO default to gathering food instead of attacking instead?
        //         if (bee.State != BeeState.Idle && bee.State != BeeState.Attacking)
        //         {
        //             return;
        //         }
        //
        //         for (int i = 0; i < yellowJacketTranslations.Length; i++)
        //         {
        //             // find the honey bee closest to this yellowjacket
        //         }
        //         // .. set bee.TargetBee, bee.TargetBeePos, and bee.TargetBeeDist
        //     // }).ScheduleParallel();
        //     }).Schedule();
        
        // TODO repeat for the honeybees
    }
}
