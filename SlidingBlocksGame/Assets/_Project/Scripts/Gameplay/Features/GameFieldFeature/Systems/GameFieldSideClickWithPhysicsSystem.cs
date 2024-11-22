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
    public class GameFieldSideClickWithPhysicsSystem : IEcsRun
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

        private readonly Camera _camera = Camera.main;

        public void Run()
        {
            foreach (int click in _world.Where(out ClickDownAspect clickDownAspect))
            {
                ref readonly ScreenPosition screenPosition = ref clickDownAspect.ScreenPositions.Read(click);

                Ray ray = _camera.ScreenPointToRay(screenPosition.Value.xyy);

#if DEBUG
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 100);
#endif

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: math.INFINITY, layerMask: 1 << 9))
                {
#if DEBUG
                    GameObject debugPoint = new GameObject
                    {
                        transform =
                        {
                            position = hit.point
                        }
                    };
#endif
                    ProcessHit(hit);
                }
            }
        }

        private void ProcessHit(RaycastHit hitInfo)
        {
            foreach (int entity in _world.Where(out GameFieldAspect gameFieldAspect))
            {
                ref GameField gameField = ref gameFieldAspect.GameFields.Get(entity);

                int2 gridPosition = GridUtils.GetCellPosition(hitInfo.point, gameField.ToGrid());
                
                if (GridUtils.IsWithinGrid(gridPosition, gameField.Size) &&
                    !GridUtils.IsWithinCenter(gridPosition, gameField.EdgeSize, gameField.CenterSize) &&
                    GridUtils.IsInCross(gridPosition, gameField.EdgeSize, gameField.CenterSize))
                {
                    int sideClick = _world.NewEntity();
                    _world.GetPool<SideClickedEvent>().Add(sideClick);
                    _world.GetPool<WorldPosition>().Add(sideClick).Value = hitInfo.point;
                    _world.GetPool<ActiveGameField>().Add(sideClick).Value = entity.ToEntityLong(_world);
                }
            }
        }
    }
}