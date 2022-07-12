using Unity.Entities;
using Unity.Transforms;

class BeeSpawnerAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject BeePrefab;
    public int BeeCount;
    public UnityEngine.Transform SpawnPoint;
}

class BeeSpawnerBaker : Baker<BeeSpawnerAuthoring>
{
    
    public override void Bake(BeeSpawnerAuthoring authoring)
    {
        // Baking happens once TODO CONFIRM
        AddComponent(new BeeSpawner()
        {
            BeePrefab = GetEntity(authoring.BeePrefab),
            BeeCount = authoring.BeeCount,
            SpawnPoint = authoring.SpawnPoint.position
        });
    }
}