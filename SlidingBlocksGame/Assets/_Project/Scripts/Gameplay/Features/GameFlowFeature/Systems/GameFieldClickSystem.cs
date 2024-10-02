using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.InputFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class GameFieldClickSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> Fields;
        }

        private class ClickDownAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ClickDownEvent))]
            [Inc] public readonly EcsPool<ScreenPosition> ScreenPositions;
        }

        private Plane _plane = new Plane(Vector3.up, Vector3.zero);

        public void Run()
        {
            foreach (int click in _world.Where(out ClickDownAspect clickDownAspect))
            {
                ref readonly ScreenPosition screenPosition = ref clickDownAspect.ScreenPositions.Read(click);

                float3 clickPosition = GetMouseClickPosition(screenPosition.Value);

                foreach (int entity in _world.Where(out Aspect aspect))
                {
                    ref GameField gameField = ref aspect.Fields.Get(entity);
                    float2 gridPosition = GridUtils.GetCellPosition(clickPosition, gameField);

                    if (IsWithinGrid(gridPosition, gameField.Size) && !GridUtils.IsWithinCenter(gridPosition, in gameField) &&
                        IsInCross(gameField, gridPosition))
                    {
                        _world.GetPool<WorldPosition>().Add(click).Value = clickPosition;
                        _world.GetPool<ActiveGameField>().Add(click).Value = entity.ToEntityLong(_world);
                        _world.GetPool<ClickSideMarker>().Add(click);
                    }
                }
            }
        }

        private Vector3 GetMouseClickPosition(float2 position)
        {
            Ray ray = Camera.main.ScreenPointToRay(position.xyy);

            return _plane.Raycast(ray, out float enter)
                ? ray.GetPoint(enter)
                : Vector3.positiveInfinity;
        }

        private bool IsWithinGrid(float2 gridPosition, int fieldSize)
        {
            return gridPosition.x >= 0 && gridPosition.x < fieldSize &&
                   gridPosition.y >= 0 && gridPosition.y < fieldSize;
        }

        private bool IsInCross(GameField field, float2 coordinates)
        {
            return (coordinates.x >= field.EdgeSize && coordinates.x < field.EdgeSize + field.CenterSize) ||
                   (coordinates.y >= field.EdgeSize && coordinates.y < field.EdgeSize + field.CenterSize);
        }
    }
}