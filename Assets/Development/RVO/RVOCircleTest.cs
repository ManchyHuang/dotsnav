using DotsNav;
using DotsNav.LocalAvoidance.Data;
using DotsNav.LocalAvoidance.Hybrid;
using DotsNav.LocalAvoidance.Systems;
using DotsNav.Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RVOCircleTest : MonoBehaviour
{
    public int AgentAmount;
    public float SpawnRadius;
    public DotsNavLocalAvoidanceAgent Prefab;
    public DotsNavLocalAvoidance LocalAvoidance;

    void Start()
    {
        for (int i = 0; i < AgentAmount; i++)
        {
            var prefab = Instantiate(Prefab);
            prefab.GetComponent<DotsNavLocalAvoidanceAgent>().LocalAvoidance = LocalAvoidance;
            var pos = SpawnRadius * new float2(math.cos(i * 2 * math.PI / AgentAmount), math.sin(i * 2 * math.PI / AgentAmount));
            prefab.transform.position = pos.ToXxY();
        }
    }
}

struct TargetComponent : IComponentData
{
    public float2 Value;
}

[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateBefore(typeof(RVOSystem))]
class DirectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.fixedDeltaTime;

        Entities
            .WithBurst()
            .ForEach((Translation translation, TargetComponent target, LocalAvoidanceSettings agent, ref PreferredVelocityComponent preferredVelocity) =>
            {
                var toTarget = target.Value - translation.Value.xz;
                var length = math.length(toTarget);
                const float preferredSpeed = 8;
                if (length >= preferredSpeed * dt)
                    preferredVelocity.Value = toTarget / length * preferredSpeed;
                else if (length >= -1e3f)
                    preferredVelocity.Value = toTarget;
                else
                    preferredVelocity.Value = 0;
            })
            .ScheduleParallel();
    }
}

[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateAfter(typeof(RVOSystem))]
class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        Entities
            .WithoutBurst()
            .ForEach((DotsNavLocalAvoidanceAgent agent) =>
            {
                agent.transform.position += (Vector3) (agent.Velocity * dt).ToXxY();
            })
            .Run();
    }
}

class TargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .WithAll<LocalAvoidanceSettings>()
            .WithNone<TargetComponent>()
            .WithStructuralChanges()
            .ForEach((Entity entity, Translation translation) =>
            {
                EntityManager.AddComponentData(entity, new TargetComponent{Value = -translation.Value.xz});
            })
            .Run();
    }
}