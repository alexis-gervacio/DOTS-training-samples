using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BeeSpawnSystem))]
public partial class BeeBehaviourSystem : SystemBase
{
    private EntityQuery availableFoodQuery;
    private EntityQuery honeyBeeQuery;
    private EntityQuery yellowJacketQuery;
    
    private EntityQuery honeyBeeHiveQuery;
    private EntityQuery yellowJacketHiveQuery;

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
            All = new ComponentType[] { typeof(Bee), typeof(FactionHoneyBee), typeof(Translation)},
            None = new ComponentType[] { typeof(Dead) }
        });
        yellowJacketQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Bee), typeof(FactionYellowJacket), typeof(Translation)},
            None = new ComponentType[] { typeof(Dead) }
        });
        
        /// Beehive entities
        honeyBeeHiveQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(BeeSpawner), typeof(FactionHoneyBee)}
        });
        
        yellowJacketHiveQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(BeeSpawner), typeof(FactionYellowJacket)}
        });
    }

    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var random = new Unity.Mathematics.Random(1234789);
        
        NativeArray<Entity> availableFoodArray = availableFoodQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> honeyBeesArray = honeyBeeQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> yellowJacketsArray = yellowJacketQuery.ToEntityArray(Allocator.TempJob);
        
        // TODO is there a way to cache the entity without having to query each time?
        // But still be able to use it in the Entities ForEach
        NativeArray<Entity> honeyBeeHiveArray = honeyBeeHiveQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> yellowJacketHiveArray = yellowJacketHiveQuery.ToEntityArray(Allocator.TempJob);
        
        Entities
            .WithAll<Bee>()
            .WithNone<Dead>()
            .WithDisposeOnCompletion(availableFoodArray)
            .WithDisposeOnCompletion(honeyBeesArray)
            .WithDisposeOnCompletion(yellowJacketsArray)
            .ForEach((Entity entity, TransformAspect transform, ref Bee bee) =>
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
                        // Debug.Log($"ASSIGN AS FOOD GATHERER");
                    }
                    else if (bee.State == Bee.BeeState.Attacking)
                    {
                        // TOdo filter to check for dead bees
                        // Debug.Log($"ASSIGN AS ATTACKER");
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
                else
                {
                    if (bee.Movement.Target != Entity.Null)
                    {
                        var pos = transform.Position;
                        var targetPosition = GetComponent<Translation>(bee.Movement.Target).Value;
                        // Debug.Log($"distance to target: {math.distance(pos, targetPosition)}");
                        // Check if reached target
                        if (math.distance(pos, targetPosition) <= 0.5f)
                        {
                            if (bee.State == Bee.BeeState.GatheringFood)
                            {
                                // Todo make food follow the holder bee movement...
                                ecb.AddComponent(bee.Movement.Target, new HolderBee() {Holder = entity});
                                // TODO REMOVE COMPONENT LATER
                                bee.State = Bee.BeeState.ReturningFood;
        
                                bee.Movement.Target = bee.Faction switch
                                {
                                    BeeFaction.HoneyBee => honeyBeeHiveArray[0],
                                    BeeFaction.YellowJacket => yellowJacketHiveArray[0],
                                };
                            }
                        }
                    }
                }
                
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
