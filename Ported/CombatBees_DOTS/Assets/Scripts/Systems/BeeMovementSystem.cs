using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BeeSpawnSystem))]
[UpdateAfter(typeof(BeeBehaviourSystem))]
partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // The Entities.ForEach below is Burst compiled (implicitly).
        // And time is a member of SystemBase, which is a managed type (class).
        // This means that it wouldn't be possible to directly access Time from there.
        // So we need to copy the value we need (DeltaTime) into a local variable.
        var dt = Time.DeltaTime;

        var field = GetSingleton<Field>();
        
        // Entities.ForEach is an older approach to processing queries. Its use is not
        // encouraged, but it remains convenient until we get feature parity with IFE.
        Entities
            .WithAll<Bee>()
            .ForEach((Entity entity, TransformAspect transform, in Bee bee) =>
            {
                var pos = transform.Position;

                // Move bee toward their target.
                // TODO need to add randomness to movement rather than the smooth lerp
                if (bee.Movement.Target != Entity.Null)
                {
                    float3 diff = GetComponent<Translation>(bee.Movement.Target).Value - pos;
                    transform.Position += diff * dt;
                }

               //  /// STUB from tanks example
               //  // This does not modify the actual position of the tank, only the point at
               //  // which we sample the 3D noise function. This way, every tank is using a
               //  // different slice and will move along its own different random flow field.
               //  pos.y = entity.Index;
               //
               //  // Unity.Mathematics.noise provides several types of noise functions.
               //  // Here we use the Classic Perlin Noise (cnoise).
               //  // The approach taken to generate a flow field from Perlin noise is detailed here:
               //  // https://www.bit-101.com/blog/2021/07/mapping-perlin-noise-to-angles/
               //  var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
               //
               //  var dir = float3.zero;
               //  math.sincos(angle, out dir.x, out dir.z);
               //  transform.Position += dir * dt * 5.0f;
               //  transform.Rotation = quaternion.RotateY(angle);
               // ////
               

                ///// TODO constrain within field
               // if (Utility.ExceedsXBounds(field, transform.Position))
               // {
               //     Debug.Log($"exceeds x bounds");
               // }
               ///


               // transform.Position += dir * dt * 5.0f;

               // The last function call in the Entities.ForEach sequence controls how the code
               // should be executed: Run (main thread), Schedule (single thread, async), or
               // ScheduleParallel (multiple threads, async).
               // Entities.ForEach is fundamentally a job generator, and it makes it very easy to
               // create parallel jobs. This unfortunately comes with a complexity cost and weird
               // arbitrary constraints, which is why more explicit approaches are preferred.
               // Those explicit approaches (IJobEntity) are covered later in this tutorial.
               // }).ScheduleParallel();
            // }).Schedule();
            }).Run();
    }
}