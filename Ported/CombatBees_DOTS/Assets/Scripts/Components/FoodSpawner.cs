using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct FoodSpawner : IComponentData
{
    public Entity FoodPrefab;
    // TODO SPAWN FOOD on mouse click 

}