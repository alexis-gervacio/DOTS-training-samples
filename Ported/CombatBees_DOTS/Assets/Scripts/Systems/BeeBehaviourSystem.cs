using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BeeSpawnSystem))]
public partial class BeeBehaviourSystem : SystemBase
{
    private EntityQuery availableFoodQuery;
    private EntityQuery honeyBeeQuery;
    private EntityQuery yellowJacketQuery;
    
    protected override void OnCreate()
    {
        // // do these queries get updated? TODO
        availableFoodQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(FoodSource), typeof(Translation)},
            None = new ComponentType[] { typeof(Dead), typeof(HolderBee) }
        });
        honeyBeeQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(FactionHoneyBee), typeof(Translation)},
            None = new ComponentType[] { typeof(Dead) }
        });
        yellowJacketQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(FactionYellowJacket), typeof(Translation)},
            None = new ComponentType[] { typeof(Dead) }
        });
    }

    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var random = new Unity.Mathematics.Random(1234789);
        
        NativeArray<Entity> availableFoodArray = availableFoodQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> honeyBeesArray = honeyBeeQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> yellowJacketsArray = yellowJacketQuery.ToEntityArray(Allocator.TempJob);

        // Debug.Log($"[{this.GetType().ToString()}] honey bee count: {honeyBeesArray.Length}");
        // Debug.Log($"[{this.GetType().ToString()}] yellowjacket count: {yellowJacketsArray.Length}");
        Entities
            .WithAll<Bee>()
            .WithNone<Dead>()
            // .WithReadOnly<NativeArray<Entity>>(availableFoodArray)
            // .WithDisposeOnCompletion(availableFoodArray)
            .ForEach((Entity entity, ref Bee bee, in Translation translation) =>
            {
                // Assign jobs to idle bees
                if (bee.State == Bee.BeeState.Idle)
                {
                    // randomize, either bee will try to attack or collect food
                    int randomJob = random.NextInt(1, 3); // Max is exclusive so increment by 1
                    bee.State = (Bee.BeeState) randomJob;

                    if (bee.State == Bee.BeeState.GatheringFood)
                    {
                        // TODO CHECK AGAIN IF FOOD IS ALREADY BEING HELD
                        int randomFood = random.NextInt(availableFoodArray.Length);
                        bee.Movement.Target = availableFoodArray[randomFood];
                        Debug.Log($"ASSIGN AS FOOD GATHERER");
                    }
                    else if (bee.State == Bee.BeeState.Attacking)
                    {
                        // TOdo filter to check for dead bees
                        Debug.Log($"ASSIGN AS ATTACKER");
                        if (bee.Faction == BeeFaction.HoneyBee)
                        {
                            int randomEnemy = random.NextInt(yellowJacketsArray.Length);
                            bee.Movement.Target = yellowJacketsArray[randomEnemy];

                        }
                        else if (bee.Faction == BeeFaction.YellowJacket)
                        {
                            int randomEnemy = random.NextInt(honeyBeesArray.Length);
                            bee.Movement.Target = honeyBeesArray[randomEnemy];
                        }
                    }
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
