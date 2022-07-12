using Unity.Entities;

class BeeAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        AddComponent<Bee>();
        
        // Store the entities of all the children authoring GameObjects having a renderer.
        // var buffer = AddBuffer<ChildrenWithRenderer>().Reinterpret<Entity>();
        // foreach (var renderer in GetComponentsInChildren<UnityEngine.MeshRenderer>())
        // {
        //     buffer.Add(GetEntity(renderer));
        // }
    }
}