using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class CellOccupancySystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(MovementCommand))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<CellOccupancyMarker> CellOccupancyMarker;
            [Opt] public readonly EcsTagPool<MovingMarker> Moving;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log(entity);

                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    aspect.CellOccupancyMarker.Add(targetID);
                    // TODO: rework
                    aspect.Moving.Del(targetID);
                }
            }
        }
    }
}