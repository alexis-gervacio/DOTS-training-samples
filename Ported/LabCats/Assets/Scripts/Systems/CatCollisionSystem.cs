using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class CatCollisionSystem : SystemBase
{
    EntityQuery m_CatQuery;

    protected override void OnCreate()
    {
        m_CatQuery = GetEntityQuery(typeof(CatTag), typeof(GridPosition), typeof(CellOffset), typeof(Direction), typeof(ColliderSize));
    }

    protected override void OnUpdate()
    {
        // Grab all the Cats and put their positions in a dyanmic buffer
        // TODO: I have no idea if this will work
        var catEntities = m_CatQuery.ToEntityArray(Allocator.Temp);
        var catPositions = m_CatQuery.ToComponentDataArray<GridPosition>(Allocator.Temp);
        var catOffsets = m_CatQuery.ToComponentDataArray<CellOffset>(Allocator.Temp);
        var catDirections = m_CatQuery.ToComponentDataArray<Direction>(Allocator.Temp);
        var catColliders = m_CatQuery.ToComponentDataArray<ColliderSize>(Allocator.Temp);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Foreach on all the mice and check if they collide
        Entities
            .WithNone<CatTag>()
            .ForEach((Entity entity, in GridPosition gridPosition, in CellOffset cellOffset, in Direction direction, in ColliderSize colliderSize) =>
            {
                var offsetX = 0;
                var offsetY = 0;

                UpdateTransformSystem.GetOffsetDirs(ref offsetX, ref offsetY, in direction);

                var mouseX = gridPosition.X + cellOffset.Value * offsetX;
                var mouseY = gridPosition.Y + cellOffset.Value * offsetY;

                // TODO: Check the distance to each cat
                for (int i = 0; i < catPositions.Length; i++)
                {

                    UpdateTransformSystem.GetOffsetDirs(ref offsetX, ref offsetY, in direction);

                    var catX = catPositions[i].X + catOffsets[i].Value * offsetX;
                    var catY = catPositions[i].Y + catOffsets[i].Value * offsetY;

                    var distance = math.distance(new float2(mouseX, mouseY), new float2(catX, catY));

                    if (distance < colliderSize.Value * .5f + catColliders[i].Value * .5f)
                    {
                        ecb.DestroyEntity(entity);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
