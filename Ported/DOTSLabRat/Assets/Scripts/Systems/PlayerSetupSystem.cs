using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace DOTSRATS
{
    public class PlayerSetupSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var randomSeed = GetSingleton<BoardSpawner>().randomSeed;

            Entities
                .WithStructuralChanges()
                .WithAll<PlayerSpawner>()
                .ForEach((Entity entity, in PlayerSpawner playerSpawner) =>
                {
                    var arrows = new NativeArray<PlacedArrow>(playerSpawner.maxArrows, Allocator.Temp);

                    for (int i = 0; i < 4; i++)
                    {
                        var color = GetPlayerColor(playerSpawner, i);
                        var player = EntityManager.Instantiate(playerSpawner.playerPrefab);
                        EntityManager.SetComponentData(player, new Player
                        {
                            playerNumber = i,
                            color = GetPlayerColor(playerSpawner, i),
                            score = 0,
                            arrowToPlace = new int2(-1, -1),
                            arrowDirection = Direction.None,
                            currentArrow = 0,
                            arrowToRemove = new int2(-1, -1),
                            arrowPlacementDelayRange = playerSpawner.AIArrowPlaceDelayRange,
                            random = Random.CreateFromIndex(randomSeed == 0 ? (uint)System.DateTime.Now.Ticks : ++randomSeed)
                        });

                        for (int j = 0; j < playerSpawner.maxArrows; j++)
                        {
                            var arrow = EntityManager.Instantiate(playerSpawner.arrowPrefab);
                            EntityManager.SetComponentData(arrow, new Translation()
                            {
                                Value = new float3(-100, 0, -100)
                            });
                            EntityManager.SetComponentData(arrow, new URPMaterialPropertyBaseColor
                            {
                                Value = new float4(color.r, color.g, color.b, color.a)
                            });
                            arrows[j] = new PlacedArrow {entity = arrow};
                        }

                        EntityManager.AddBuffer<PlacedArrow>(player).AddRange(arrows);
                    }

                    EntityManager.DestroyEntity(entity);
                    arrows.Dispose();
                }).Run();
        }

        UnityEngine.Color GetPlayerColor(PlayerSpawner playerSpawner, int playerNumber)
        {
            switch (playerNumber)
            {
                case 0: return playerSpawner.player1Color;
                case 1: return playerSpawner.player2Color;
                case 2: return playerSpawner.player3Color;
                case 3: return playerSpawner.player4Color;
            }

            return default;
        }
    }
}
