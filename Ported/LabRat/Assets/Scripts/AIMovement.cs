using Unity.Entities;
using Unity.Mathematics;

public class AIMovement : SystemBase
{
    static readonly float AISpeed = 6f;
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var randomSeed = (uint)System.DateTime.Now.Ticks;
        var boardSize = GetSingleton<GameInfo>().boardSize;

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithAll<Timer, AICursor>().ForEach((Entity e, int entityInQueryIndex, in Timer timer) =>
        {
            if (timer.Value <= 0f)
                ecb.RemoveComponent<Timer>(entityInQueryIndex, e);
        }).ScheduleParallel();

        Entities.WithNone<Timer>().ForEach((Entity e, int entityInQueryIndex, ref AICursor cursor, ref Position position) =>
        {
            var direction = cursor.Destination - position.Value;
            var distance = math.lengthsq(direction);
            var movement = deltaTime * AISpeed * math.normalize(direction);
            var movementLength = math.lengthsq(movement);

            if (distance == 0 || movementLength >= distance)
            {
                position.Value = cursor.Destination;
                var random = new Random((uint)(randomSeed + entityInQueryIndex));
                ecb.AddComponent(entityInQueryIndex, e, new Timer() { Value = random.NextFloat(2f, 5f) });
                ecb.AddComponent(entityInQueryIndex, e, new PlacingArrow()
                {
                    TileIndex = (int)math.round(position.Value.x) + (int)math.round(position.Value.y) * boardSize.y,
                    Direction = (byte)(1 << random.NextInt(0, 4)),
                });
                cursor.Destination = new int2(random.NextInt(0, boardSize.x), random.NextInt(0, boardSize.y));
            }
            else
            {
                position.Value += movement;
            }

        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}