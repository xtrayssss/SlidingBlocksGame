using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CrossGrid
    {
        public static Vector2Int WorldToGridPosition(Vector3 worldPosition, GameField field)
        {
            int x = Mathf.FloorToInt(
                (worldPosition.x - field.OriginPosition.x + field.CellSize * 0.5f + field.Offset * 0.5f) /
                (field.CellSize + field.Offset));
    
            int z = Mathf.FloorToInt(
                (worldPosition.z - field.OriginPosition.z + field.CellSize * 0.5f + field.Offset * 0.5f) /
                (field.CellSize + field.Offset));

            Debug.Log(x + " " + z);
            return new Vector2Int(x, z);
        }
        
        public static float3 GetWorldPosition(float2 coordinates, GameField field) =>
            new float3(coordinates.x * (field.CellSize + field.Offset) + field.OriginPosition.x, 0,
                coordinates.y * (field.CellSize + field.Offset) + field.OriginPosition.z);

    }

    public class DetermineClickSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> Fields;
        }

        private Plane _plane = new Plane(Vector3.up, Vector3.zero);
        private readonly CrossGrid _crossGrid = new CrossGrid();

        public void Run()
        {
            if (Input.GetMouseButtonDown(0))
            {
                float3 clickPosition = GetMouseClickPosition();

                foreach (int entity in _world.Where(out DetermineClickSystem.Aspect aspect))
                {
                    ref GameField field = ref aspect.Fields.Get(entity);
                    Vector2Int gridPosition = CrossGrid.WorldToGridPosition(clickPosition, field);

                    if (IsWithinGrid(gridPosition, field.Size) && !IsCentralTile(gridPosition, field) &&
                        IsInCross(field, gridPosition.x, gridPosition.y))
                    {
                        entlong click = _world.NewEntityLong();

                        _world.GetPool<WorldPosition>().Add(click.ID).Value = clickPosition;
                        _world.GetTagPool<ClickTag>().Add(click.ID);
                        _world.GetTagPool<DeleteEntityCommand>().Add(click.ID);
                    }
                }
            }
        }

        private Vector3 GetMouseClickPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            return _plane.Raycast(ray, out float enter)
                ? ray.GetPoint(enter)
                : Vector3.positiveInfinity;
        }

        private bool IsWithinGrid(Vector2Int gridPosition, int fieldSize)
        {
            return gridPosition.x >= 0 && gridPosition.x < fieldSize &&
                   gridPosition.y >= 0 && gridPosition.y < fieldSize;
        }

        bool IsInCross(GameField field, int x, int z)
        {
            return (x >= field.EdgeSize && x < field.EdgeSize + field.CenterSize) ||
                   (z >= field.EdgeSize && z < field.EdgeSize + field.CenterSize);
        }

        private bool IsCentralTile(Vector2Int gridPosition, GameField field)
        {
            int gridCenter = field.Size / 2;
            return gridPosition.x >= gridCenter - 1 && gridPosition.x <= gridCenter &&
                   gridPosition.y >= gridCenter - 1 && gridPosition.y <= gridCenter;
        }
    }
}