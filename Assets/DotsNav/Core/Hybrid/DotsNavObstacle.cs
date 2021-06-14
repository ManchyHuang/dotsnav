using System.Linq;
using DotsNav.Editor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DotsNav.Hybrid
{
    class ObstacleConversionSystem : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((DotsNavObstacle obstacle) =>
            {
                var entity = GetPrimaryEntity(obstacle);
                obstacle.Entity = entity;
                obstacle.World = DstEntityManager.World;
            });
        }
    }

    public class DotsNavObstacle : EntityLifetimeBehaviour
    {
        /// <summary>
        /// The vertices of this obstacle in object space
        /// </summary>
        public Vector2[] Vertices;

        [Header("Debug")]
        public bool DrawGizmos = true;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (Application.isPlaying || !DrawGizmos || Selection.gameObjects.Contains(gameObject))
                return;

            var t = Handles.color;
            Handles.color = DotsNavPrefs.GizmoColor;

            for (int i = 0; i < Vertices.Length - 1; i++)
            {
                var a = math.transform(transform.localToWorldMatrix, Vertices[i].ToXxY());
                var b = math.transform(transform.localToWorldMatrix, Vertices[i + 1].ToXxY());
                Handles.DrawLine(a, b);
            }

            Handles.color = t;
        }

        void OnValidate()
        {
            if (Vertices == null || Vertices.Length == 0)
                Vertices = new[] {new Vector2(-1, 0), new Vector2(1, 0)};
            else if (Vertices.Length == 1)
                Vertices = new[] {Vertices[0], new Vector2(1, 0)};
        }
#endif
    }
}