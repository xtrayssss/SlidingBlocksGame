using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameField : IEcsComponent
    {
        [FormerlySerializedAs("TilePrefab")] public GameObject CellPrefab;
        public float3 OriginPosition;
        public int Size;
        public float Offset;
        public float CellSize;
        public int CenterSize;
        public int EdgeSize;

        public Cell[] Cells;
        public Unit[] Units;

        public int BaseSize;
        public float BaseCellSize;
        public int CellsCount;
        public float CellTop;
        [FormerlySerializedAs("CellTopOffset")] public float UnitCellTopOffset;

        [Serializable]
        public struct Unit
        {
            public float3 Position;
            public float2 CellPosition;
            public EcsEntityConnect Prefab;
            public GameObject View;
            public float3 Rotation;
        }
        
        [Serializable]
        public struct Cell
        {
            public float2 CellPosition;
            public float3 WorldPosition;
            public GameObject View;
        }

        public class Template : ComponentTemplate<GameField>
        {
#if UNITY_EDITOR
            public override void OnValidate(Object obj)
            {
                Span<Unit> units = new Span<Unit>(component.Units);

                component.CellTop = GetUpperSurfaceY(component.CellPrefab.GetComponentInChildren<MeshRenderer>());

                float GetUpperSurfaceY(MeshRenderer renderer)
                {
                    float lowerY = renderer.bounds.center.y - renderer.bounds.extents.y;
                    float upperY = lowerY + renderer.bounds.size.y;
                    return upperY;
                }
                
                foreach (ref var unit in units)
                {
                    unit.Position = CellToWorld(unit.CellPosition, component) + new float3(0, component.CellTop + component.UnitCellTopOffset, 0);
                }
            }

            private float3 CellToWorld(float2 position, GameField field)
            {
                return new float3(
                    position.x * (field.CellSize + field.Offset) + field.OriginPosition.x,
                    0,
                    position.y * (field.CellSize + field.Offset) + field.OriginPosition.z);
            }
#endif
        }
    }
}