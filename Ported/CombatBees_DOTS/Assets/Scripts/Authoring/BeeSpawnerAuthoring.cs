using Unity.Entities;
using Unity.Transforms;

class BeeSpawnerAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject BeePrefab;
    public int BeeCount;
    // public BeeFaction Faction;
    public UnityEngine.Transform SpawnPoint;
}

class BeeSpawnerBaker : Baker<BeeSpawnerAuthoring>
{
    public override void Bake(BeeSpawnerAuthoring authoring)
    {
        AddComponent(new BeeSpawner()
        {
            BeePrefab = GetEntity(authoring.BeePrefab),
            BeeCount = authoring.BeeCount,
            // Bake the spawn position
            SpawnPoint = authoring.SpawnPoint.position,
            // Faction = authoring.Faction
        });
    }
}