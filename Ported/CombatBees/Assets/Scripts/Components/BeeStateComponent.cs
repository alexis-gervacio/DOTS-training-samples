using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BeeStateComponent : IComponentData
{
    public enum BeeState
    {
        NoTarget,
        ChaseEnemy,
        AttackEnemy,
        ChaseResource,
        GrabResource,
        CarryResource,
        Dead
    }

    public BeeState Value;
}
