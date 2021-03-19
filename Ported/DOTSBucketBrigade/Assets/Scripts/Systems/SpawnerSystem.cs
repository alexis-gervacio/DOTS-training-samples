﻿using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity heatMapEntity = EntityManager.CreateEntity();
        EntityManager.AddComponent<HeatMapTag>(heatMapEntity);
        DynamicBuffer<HeatMap> heatMap = EntityManager.AddBuffer<HeatMap>(heatMapEntity);
        
        Entities
            .ForEach((Entity entity, in InitCounts initCounts) =>
            {
                heatMap.Length = initCounts.GridSize * initCounts.GridSize;
                int i = 0;
                
                for (int row = 0; row < initCounts.GridSize; ++row)
                {
                    for (int col = 0; col < initCounts.GridSize; ++col)
                    {
                        var instance = ecb.Instantiate(initCounts.CellPrefab);
                        var translation = new Translation 
                        { 
                            Value = new float3(col, 0, row) 
                        };
                        heatMap[i] = new HeatMap {Value = 0.0f};
                        
                        ecb.SetComponent(instance, translation);
                        i++;
                    }
                }

                for (int buckets = 0; buckets < initCounts.InitialBucketCount; buckets++)
                {
                    var instance = ecb.Instantiate(initCounts.BucketPrefab);
                    var translation = new Translation 
                    { 
                        Value = new float3(Random.Range(1,initCounts.GridSize-1), 0.0f, Random.Range(1,initCounts.GridSize-1)) 
                    };
                    ecb.SetComponent(instance, translation);
                }
                
                for (int waters = 0; waters < initCounts.WaterSourceCount; waters++)
                {
                    int side = Random.Range(0, 4);     
                    var instance = ecb.Instantiate(initCounts.WaterPrefab);
                    float x, z;
                    switch (side)
                    {
                        case 0:
                            x = Random.Range(1.0f, initCounts.GridSize - 2.0f);
                            z = Random.Range(-6.0f, -2.0f);
                            break;
                        case 1:
                            x = Random.Range(1.0f, initCounts.GridSize - 2.0f);
                            z = Random.Range(initCounts.GridSize + 2.0f, initCounts.GridSize + 6.0f);
                            break;
                        case 2:
                            x = Random.Range(-6.0f, -2.0f);
                            z = Random.Range(1.0f, initCounts.GridSize - 2.0f);
                            break;
                        default:
                            x = Random.Range(initCounts.GridSize + 2.0f, initCounts.GridSize + 6.0f);
                            z = Random.Range(1.0f, initCounts.GridSize - 2.0f);
                            break;
                    }
                    
                    var translation = new Translation 
                    { 
                        Value = new float3(x, 0.0f, z)
                    };
                    ecb.SetComponent(instance, translation);
                }

                for (int bucketGroups = 0; bucketGroups < initCounts.BucketChainCount; bucketGroups++)
                {
                    var line = new Line();
                    var location = new float3(Random.Range(1, initCounts.GridSize - 1), 0.0f,
                        Random.Range(1, initCounts.GridSize - 1));
                    var fetcher = ecb.Instantiate(initCounts.FetcherPrefab);
                    var fetcherTranslation = new Translation
                    {
                        Value = location
                    };
                    ecb.SetComponent(fetcher, fetcherTranslation);

                    var halfChain = initCounts.WorkerCountPerChain / 2;
                    Entity previousBot = default;
                    line.HalfCount = halfChain;
                    var lineEntity = ecb.CreateEntity();
                    for (int bucketers = 0; bucketers < halfChain; bucketers++)
                    {
                        var bot = ecb.Instantiate(initCounts.BotPrefab);
                        if (bucketers == 0)
                        {
                            line.EmptyHead = bot;
                        }

                        var translation = new Translation
                        {
                            Value = location
                        };
                        ecb.SetComponent(bot, translation);
                        if (previousBot != Entity.Null)
                        {
                            ecb.SetComponent(previousBot, new NextPerson {Value = bot});
                        }
                        ecb.AddComponent<EmptyBucketer>(bot);
                        if (bucketers == halfChain - 1)
                        {
                            ecb.AddComponent<LastInLine>(bot);
                            ecb.AddComponent<CurrentLine>(bot);
                            ecb.SetComponent(bot,new CurrentLine(){Value = lineEntity});
                            line.EmptyTail = bot;
                        }
                        previousBot = bot;
                    }

                    previousBot = default;
                    for (int bucketers = 0; bucketers < halfChain; bucketers++)
                    {
                        var bot = ecb.Instantiate(initCounts.BotPrefab);
                        if (bucketers == 0)
                        {
                            line.FullHead = bot;
                        }
                        var translation = new Translation
                        {
                            Value = location
                        };
                        ecb.SetComponent(bot, translation);
                        if (previousBot != Entity.Null)
                        {
                            ecb.SetComponent(previousBot, new NextPerson {Value = bot});
                        }
                        ecb.AddComponent<FullBucketer>(bot);
                        if (bucketers == halfChain - 1)
                        {
                            ecb.AddComponent<LastInLine>(bot);
                            ecb.AddComponent<CurrentLine>(bot);
                            ecb.SetComponent<CurrentLine>(bot, new CurrentLine() {Value = lineEntity});
                            line.FullTail = bot;
                        }
                        previousBot = bot;
                    }
                    ecb.AddComponent<Line>(lineEntity);
                    ecb.SetComponent<Line>(lineEntity, line);
                    ecb.AddComponent<Reposition>(lineEntity);
                }
                
                Unity.Mathematics.Random fireRandomizer = new Unity.Mathematics.Random((uint) Random.Range(1, heatMap.Length));
                for (int fireCount = 0; fireCount < initCounts.InitialFireInstances; ++fireCount)
                {
                    int fireIndex = fireRandomizer.NextInt(initCounts.GridSize * initCounts.GridSize);
                    heatMap[fireIndex] = new HeatMap {Value = 1.0f};
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        this.Enabled = false;  
    }
}
