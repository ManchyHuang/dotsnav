﻿using DotsNav.Data;
using DotsNav.LocalAvoidance.Data;
using DotsNav.LocalAvoidance.Systems;
using DotsNav.PathFinding.Data;
using DotsNav.PathFinding.Systems;
using DotsNav.Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateAfter(typeof(PathFinderSystem))]
[UpdateBefore(typeof(RVOSystem))]
class PreferredVelocitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithBurst()
            .ForEach((Translation translation, DirectionComponent direction, SteeringComponent steering, PathQueryComponent query, ref PreferredVelocityComponent preferredVelocity) =>
            {
                var dist = math.length(query.To - translation.Value);
                var speed = math.min(dist * steering.BrakeSpeed, steering.PreferredSpeed);
                preferredVelocity.Value = direction.Value * speed;
            })
            .ScheduleParallel();
    }
}