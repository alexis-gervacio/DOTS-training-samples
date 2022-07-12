using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class BeeBehaviourSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach(
            (ref Bee Bee, ref BeeMovement movement, in Translation translation) =>
            {
                // handle each bee state
                switch (Bee.State)
                {
                    case Bee.BeeState.GatheringFood:
                    {
                        break;
                    }
                    case Bee.BeeState.ReturningFood:
                    {
                        // go back to hive
                        break;
                    }
                    case Bee.BeeState.Attacking:
                    {
                        break;
                    }
                };
            }).Run();
        // ecb.Playback();
    }
}
