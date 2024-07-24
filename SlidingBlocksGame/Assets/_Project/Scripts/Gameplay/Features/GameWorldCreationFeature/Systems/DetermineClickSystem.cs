using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class DetermineClickSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> Fields;
        }

        public Plane plane = new Plane(Vector3.up, Vector3.zero); // Плоскость на уровне y = 0

        private Vector3 GetMouseClickPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }

            return Vector3.positiveInfinity; // Возвращает бесконечность, если пересечения нет
        }

        public void Run()
        {
            Update();
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse click
            {
                Vector3 clickPosition = GetMouseClickPosition();
                foreach (int entity in _world.Where(out Aspect aspect))
                {
                    ref GameField field = ref aspect.Fields.Get(entity);
                    Vector2Int gridPosition = WorldToGridPosition(clickPosition, field);

                    if (IsWithinGrid(gridPosition, field.Size) && !IsCentralTile(gridPosition, field))
                    {
                        Vector2Int direction = GetClickDirection(gridPosition, field);
                        Debug.Log("Click direction: " + direction);
                    }
                }
            }
        }

        private Vector2Int WorldToGridPosition(Vector3 worldPosition, GameField field)
        {
            int x = Mathf.FloorToInt((worldPosition.x - field.OriginPosition.x) / (field.CellSize + field.Offset));
            int z = Mathf.FloorToInt((worldPosition.z - field.OriginPosition.z) / (field.CellSize + field.Offset));
            return new Vector2Int(x, z);
        }

        private bool IsWithinGrid(Vector2Int gridPosition, int fieldSize)
        {
            return gridPosition.x >= 0 && gridPosition.x < fieldSize &&
                   gridPosition.y >= 0 && gridPosition.y < fieldSize;
        }

        private bool IsCentralTile(Vector2Int gridPosition, GameField field)
        {
            int gridCenter = field.Size / 2;
            return gridPosition.x >= gridCenter - 1 && gridPosition.x <= gridCenter &&
                   gridPosition.y >= gridCenter - 1 && gridPosition.y <= gridCenter;
        }

        private Vector2Int GetClickDirection(Vector2Int gridPosition, GameField field)
        {
            Vector2Int gridCenter = new Vector2Int(field.Size / 2, field.Size / 2);
            return new Vector2Int(gridPosition.x - gridCenter.x, gridPosition.y - gridCenter.y);
        }
    }
}