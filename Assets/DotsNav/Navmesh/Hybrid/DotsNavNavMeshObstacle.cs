﻿using DotsNav.Data;
using DotsNav.Hybrid;
using DotsNav.Navmesh.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DotsNav.Navmesh.Hybrid
{
    [UpdateAfter(typeof(NavmeshConversionSystem))]
    class NavMeshObstacleConversionSystem : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((DotsNavNavMeshObstacle obstacle) =>
            {
                var entity = GetPrimaryEntity(obstacle);
                DstEntityManager.AddComponentData(entity, new ObstacleComponent());
                var values = DstEntityManager.AddBuffer<VertexElement>(entity);
                var o = obstacle.transform.GetComponent<DotsNavObstacle>();

                for (int i = 0; i < o.Vertices.Length; i++)
                    values.Add((float2)o.Vertices[i]);
            });
        }
    }

    /// <summary>
    /// Create to triggers insertion of a navmesh obstacle. Destroy to trigger removal of a navmesh obstacle.
    /// </summary>
    [RequireComponent(typeof(DotsNavObstacle))]
    public class DotsNavNavMeshObstacle : MonoBehaviour
    {
    }
}