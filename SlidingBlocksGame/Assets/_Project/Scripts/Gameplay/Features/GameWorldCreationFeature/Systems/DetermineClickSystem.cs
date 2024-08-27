using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
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

        private Plane _plane = new Plane(Vector3.up, Vector3.zero);

        public void Run()
        {
            if (Input.GetMouseButtonDown(0))
            {
                float3 clickPosition = GetMouseClickPosition();

                foreach (int entity in _world.Where(out Aspect aspect))
                {
                    ref GameField field = ref aspect.Fields.Get(entity);
                    float2 gridPosition = GameFieldUtils.WorldToGridPosition(clickPosition, field);

                    if (IsWithinGrid(gridPosition, field.Size) && !IsCentralTile(gridPosition, field) &&
                        IsInCross(field, gridPosition))
                    {
                        entlong click = _world.NewEntityLong();

                        _world.GetPool<WorldPosition>().Add(click.ID).Value = clickPosition;
                        _world.GetTagPool<ClickTag>().Add(click.ID);
                        UnityEngine.Debug.Log( _world.GetPool<DeleteEntityCommand>().Has(click.ID));
                        _world.GetTagPool<DeleteEntityCommand>().Add(click.ID);
                        _world.GetPool<ActiveGameField>().Add(click.ID).Value = entity.ToEntityLong(_world);
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

        private bool IsWithinGrid(float2 gridPosition, int fieldSize)
        {
            return gridPosition.x >= 0 && gridPosition.x < fieldSize &&
                   gridPosition.y >= 0 && gridPosition.y < fieldSize;
        }

        bool IsInCross(GameField field, float2 coordinates)
        {
            return (coordinates.x >= field.EdgeSize && coordinates.x < field.EdgeSize + field.CenterSize) ||
                   (coordinates.y >= field.EdgeSize && coordinates.y < field.EdgeSize + field.CenterSize);
        }

        private bool IsCentralTile(float2 coordinates, GameField field)
        {
            int gridCenter = field.Size / 2;
            
            return coordinates.x >= gridCenter - 1 && coordinates.x <= gridCenter &&
                   coordinates.y >= gridCenter - 1 && coordinates.y <= gridCenter;
        }
    }
}