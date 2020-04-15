﻿using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PercentCompleteSystem))]
public class RenderPositionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfo)));
    }

    protected override void OnUpdate()
    {
        var laneInfo = GetSingleton<LaneInfo>();

        Entities.ForEach((ref Translation translation, in PercentComplete percentComplete, in LaneAssignment laneAssignment) =>
        {
            float xPos = ((laneInfo.StartXZ.x - laneInfo.EndXZ.y) / 4) * laneAssignment.Value;
            float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.x) * percentComplete.Value;
            translation.Value.x = xPos;
            translation.Value.z = yPos;
        }).ScheduleParallel();
    }
}
