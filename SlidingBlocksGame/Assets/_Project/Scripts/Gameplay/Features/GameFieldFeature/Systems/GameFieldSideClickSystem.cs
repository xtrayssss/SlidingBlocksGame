using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems
{
    public class GameFieldSideClickSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class ClickDownAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ClickDownEvent))]
            [Inc] public readonly EcsPool<ScreenPosition> ScreenPositions;
        }

        private Plane _plane = new Plane(inNormal: Vector3.up, inPoint: Vector3.zero);

        private static readonly Camera CAMERA = Camera.main;

        public void Run()
        {
            foreach (int click in _world.Where(out ClickDownAspect clickDownAspect))
            {
                ref readonly ScreenPosition screenPosition = ref clickDownAspect.ScreenPositions.Read(click);

                float3 clickPosition = GetMouseClickPosition(screenPosition.Value);

                foreach (int entity in _world.Where(out GameFieldAspect gameFieldAspect))
                {
                    ref GameField gameField = ref gameFieldAspect.GameFields.Get(entity);
                    int2 gridPosition = GridUtils.GetCellPosition(clickPosition, gameField.ToGrid());

                    Debug.Log(clickPosition);
                    if (GridUtils.IsWithinGrid(gridPosition, gameField.Size) &&
                        !GridUtils.IsWithinCenter(gridPosition, gameField.EdgeSize, gameField.CenterSize) &&
                        GridUtils.IsInCross(gridPosition, gameField.EdgeSize, gameField.CenterSize))
                    {
                        int sideClick = _world. NewEntity();
                        _world.GetPool<SideClickedEvent>().Add(sideClick);
                        _world.GetPool<WorldPosition>().Add(sideClick).Value = clickPosition;
                        _world.GetPool<ActiveGameField>().Add(sideClick).Value = entity.ToEntityLong(_world);
                    }
                }
            }
        }

        private Vector3 GetMouseClickPosition(float2 position)
        {
            Ray ray = CAMERA.ScreenPointToRay(position.xyy);

            return _plane.Raycast(ray, out float enter)
                ? ray.GetPoint(enter)
                : Vector3.positiveInfinity;
        }
    }
}