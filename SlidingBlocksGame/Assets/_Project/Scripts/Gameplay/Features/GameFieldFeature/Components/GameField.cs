using System;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameField : IEcsComponent
    {
        public GameObject CellPrefab;

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
        public float CellScaleY;

        public float UnitCellTopOffset;
        public short Center;
        public float BaseCellScaleY;

        [Serializable]
        public struct AnimalsData
        {
            public float3 Position;
            public int2 CellPosition;
            public quaternion Rotation;
            public int2 InvertedSide;
            public float3 Scale;
        }

        [Serializable]
        public struct Cell
        {
            public float3 WorldPosition;
            public GameObject View;
        }

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components",
            sourceClassName: "GameField/Template", sourceAssembly: "Assembly-CSharp")]
        public class Template : ComponentTemplate<GameField>
        {
#if UNITY_EDITOR
            private static readonly float3 Upward = new float3(0, 1, 0);

            public override void OnValidate(Object obj)
            {
                component.EdgeSize = component.Size / 3;
                component.CenterSize = component.Size - 2 * component.EdgeSize;

                component.Cells = new Cell[component.CellsCount];

                float initialZoneSize = component.BaseSize * component.BaseCellSize;

                float newTileSize = initialZoneSize / component.Size;

                component.CellSize = newTileSize;

                foreach (ref AnimalsData animal in component.Animals.AsSpan())
                {
                    animal.Position = CellToWorld(animal.CellPosition, component);

                    animal.InvertedSide = GridUtils.GetInvertedSide(animal.CellPosition, in component);

                    animal.Rotation = quaternion.LookRotation(
                        forward: new float3(animal.InvertedSide.x, 0, animal.InvertedSide.y),
                        up: Upward);

                    animal.Scale = component.CellSize - 0.1f;
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