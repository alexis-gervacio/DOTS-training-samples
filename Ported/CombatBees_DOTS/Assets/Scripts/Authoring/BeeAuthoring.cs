using Unity.Entities;

class BeeAuthoring : UnityEngine.MonoBehaviour
{
    public BeeFaction Faction;
}

class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        AddComponent<Bee>(new Bee() {Faction = authoring.Faction});
        
        /// TODO kind of redundant but we use both a field within Bee, and the tags for factions
        /// Could just consolidate into 1
        switch (authoring.Faction)
        {
            case BeeFaction.HoneyBee:
                AddComponent<FactionHoneyBee>();
                break;
            case BeeFaction.YellowJacket:
                AddComponent<FactionYellowJacket>();
                break;
        }
        
        // Store the entities of all the children authoring GameObjects having a renderer.
        // var buffer = AddBuffer<ChildrenWithRenderer>().Reinterpret<Entity>();
        // foreach (var renderer in GetComponentsInChildren<UnityEngine.MeshRenderer>())
        // {
        //     buffer.Add(GetEntity(renderer));
        // }
    }
}