using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

namespace DOTSRATS
{
    public class PlayerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<GameState>();
            RequireSingletonForUpdate<CellStruct>();
        }

        protected override void OnUpdate()
        {
            var gameStateEntity = GetSingletonEntity<GameState>();
            var cellStructs = GetBuffer<CellStruct>(gameStateEntity);
            var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);
            var elapsedTime = Time.ElapsedTime;
            
            Entities
                .WithAll<InPlay>()
                .WithoutBurst()
                .ForEach((ref Player player, in DynamicBuffer<PlacedArrow> placedArrows) =>
                {
                    //Handle player AI system
                    if (player.playerNumber != 0)
                    {
                        if (player.nextArrowTime < elapsedTime)
                        {
                            int cellIndex;
                            CellStruct cell;
                            do
                            {
                                player.arrowToPlace = new int2(player.random.NextInt(0, gameState.boardSize), player.random.NextInt(0, gameState.boardSize));
                                cellIndex = player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x;
                                cell = cellStructs[cellIndex]; 
                                // Look for a coordinate that's not a hole, goal and does not have an arrow placed 
                            } while (cell.hole || cell.goal != default || cell.arrow != Direction.None);
                            player.arrowDirection = Utils.GetRandomCardinalDirection(ref player.random);
                            
                            if (player.nextArrowTime != 0)
                            {
                                cellIndex = player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x;
                                cell = cellStructs[cellIndex];
                                cell.arrow = player.arrowDirection;
                                cellStructs[cellIndex] = cell;
                                
                                Debug.Log($"AI {player.playerNumber} placed arrow {(int)cell.arrow} at {player.arrowToPlace}");
                            }
                            
                            var arrowDelayRange = player.arrowPlacementDelayRange;
                            player.nextArrowTime = elapsedTime + player.random.NextFloat(arrowDelayRange.x, arrowDelayRange.y);
                            Debug.Log($"AI {player.playerNumber} will place next arrow after {player.nextArrowTime - elapsedTime}");
                        }
                    }

                    //Validate placed arrow
                    if (player.arrowToPlace.x != -1)
                    {
                        for (int i = 0; i < placedArrows.Length; i++)
                        {
                            var arrowTranslation = EntityManager.GetComponentData<Translation>(placedArrows[i].entity);
                            if ((int) arrowTranslation.Value.x == player.arrowToPlace.x &&
                                (int) arrowTranslation.Value.z == player.arrowToPlace.y)
                            {
                                player.arrowToPlace = new int2(-1, -1);
                                break;
                            }
                        }
                    }

                    //Place new arrow
                    if (player.arrowToPlace.x != -1)
                    {
                        CellStruct cell;
                        int cellIndex;
                        
                        //remove oldest arrow in board struct
                        if (player.arrowToRemove.x != -1)
                        {
                            cellIndex = player.arrowToRemove.y * gameState.boardSize + player.arrowToRemove.x;
                            cell = cellStructs[cellIndex];
                            cell.arrow = Direction.None;
                            cellStructs[cellIndex] = cell;
                        }

                        //update cell in board struct with newly placed arrow
                        cellIndex = player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x;
                        cell = cellStructs[cellIndex];
                        cell.arrow = player.arrowDirection;
                        cellStructs[cellIndex] = cell;
                        
                        //update cell in board struct with newly placed arrow
                        Entity currentArrow = placedArrows[player.currentArrow].entity;
                        EntityManager.SetComponentData(currentArrow, new Translation() { Value = new float3(player.arrowToPlace.x, 0.05f, player.arrowToPlace.y) });

                        //set arrow rotation
                        quaternion rotation = quaternion.identity;
                        switch (player.arrowDirection)
                        {
                            case Direction.North:
                                rotation = new quaternion(0.7071068f,0,0,0.7071068f);
                                break;
                            case Direction.East:
                                rotation = new quaternion(0.5f,0.5f,-0.5f,0.5f);
                                break;
                            case Direction.South:
                                rotation = new quaternion(0,0.7071068f,-0.7071068f,0);
                                break;
                            case Direction.West:
                                rotation = new quaternion(-0.5f,0.5f,-0.5f,-0.5f);
                                break;
                        }
                        EntityManager.SetComponentData(currentArrow, new Rotation() { Value = rotation });
                        
                        //update arrow index
                        player.currentArrow++;
                        if (player.currentArrow == placedArrows.Length)
                            player.currentArrow = 0;
                        
                        //check if all arrows are placed, and mark next one to be removed
                        currentArrow = placedArrows[player.currentArrow].entity;
                        var oldestPos = EntityManager.GetComponentData<Translation>(currentArrow);
                        if (oldestPos.Value.x > 0)
                            player.arrowToRemove = new int2((int) oldestPos.Value.x, (int) oldestPos.Value.z);

                        player.arrowToPlace = new int2(-1, -1);
                    }
                }).Run();
        }
    }
}
