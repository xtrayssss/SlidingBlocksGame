using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameField : IEcsComponent
    {
        public GameObject CellPrefab;
        public EcsEntityConnect AnimalPrefab;

        public float3 OriginPosition;
        public int Size;
        public float Offset;
        public float CellSize;
        public int CenterSize;
        public int EdgeSize;

        public Cell[] Cells;
        public AnimalsData[] Animals;

        public int BaseSize;
        public float BaseCellSize;
        public int CellsCount;

        [HideInInspector]
        public float CellTop;

        public float UnitCellTopOffset;

        [Serializable]
        public struct AnimalsData
        {
            public float3 Position;
            public int2 CellPosition;
            public EcsEntityConnect View;
            public quaternion Rotation;
            public int2 InvertedSide;
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
            private static readonly float3 Upward = new float3(0, 1, 0);

            public override void OnValidate(Object obj)
            {
                component.CellTop = GetUpperSurfaceY(component.CellPrefab.GetComponentInChildren<MeshRenderer>());

                component.EdgeSize = component.Size / 3;
                component.CenterSize = component.Size - 2 * component.EdgeSize;

                component.Cells = new Cell[component.CellsCount];

                float initialZoneSize = component.BaseSize * component.BaseCellSize;

                float newTileSize = initialZoneSize / component.Size;

                component.CellSize = newTileSize;

                float GetUpperSurfaceY(MeshRenderer renderer)
                {
                    float lowerY = renderer.bounds.center.y - renderer.bounds.extents.y;
                    float upperY = lowerY + renderer.bounds.size.y;
                    return upperY;
                }


                foreach (ref AnimalsData animal in component.Animals.AsSpan())
                {
                    animal.Position = CellToWorld(animal.CellPosition, component) +
                                      new float3(0, component.CellTop + component.UnitCellTopOffset, 0);

                    animal.InvertedSide = GridUtils.GetInvertedSide(animal.CellPosition, in component);

                    animal.Rotation = quaternion.LookRotation(
                        forward: new float3(animal.InvertedSide.x, 0, animal.InvertedSide.y),
                        up: Upward);
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