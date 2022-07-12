using Unity.Entities;
using Unity.Mathematics;

struct Bee : IComponentData
{
    // public BeeFaction Faction;
    public BeeState State;
    public BeeMovement Movement;
    
    public enum BeeState
    {
        Idle = 0,
        GatheringFood,
        /// <summary>
        /// ALX: if returning food, can we assume that bee has successfully gotten resource and
        /// cannot drop it unless they are killed by other faction
        /// </summary>
        ReturningFood,
        Attacking
    }

    public enum BeeFaction
    {
        YellowJacket,
        HoneyBee
    }
}

struct BeeMovement : IComponentData
{
    /// <summary>
    ///  Destination could be attack target, home hive, food resource positions
    /// </summary>
    public float3 Destination;
    public float Speed;
    public float3 LogicalPosn;
    public float3 PerlinOffset;
}

struct FactionYellowJacket : IComponentData
{
}

struct FactionHoneyBee : IComponentData
{
}