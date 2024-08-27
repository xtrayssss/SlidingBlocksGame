using System.Runtime.InteropServices;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.c;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class LevelLostCheckSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [ExcImplicit(typeof(LevelWonMarker))]
            [Exc] public readonly EcsTagPool<LevelLostEvent> LevelLostEvent;

            [Exc] public readonly EcsTagPool<LevelLostMarker> LevelLostMarker;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<CooldownExpiredEvent> _cooldownExpiredEvents;
            [Inc] private readonly EcsTagPool<GameLossTimerTag> _gameLossTimerTag;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<CellOccupancyMarker> _cellOccupancyMarkers;
            [Exc] private readonly EcsTagPool<WithinCenterMarker> _withinCenterMarkers;
        }
        
        private class MovingAnimals : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<MovingMarker> MovingMarkers;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out GameLossTimerAspect _))
            {
                foreach (int entity in _world.Where(out LevelAspect aspect))
                {
                    aspect.LevelLostEvent.Add(entity);
                    aspect.LevelLostMarker.Add(entity);
                }
            }

            foreach (int entity in _world.Where(out LevelAspect aspect))
            {
                if (_world.Where(out AnimalAspect _).Count != 0 && _world.Where(out MovingAnimals _).Count == 0)
                {
                    EcsDebug.Break();
                    aspect.LevelLostEvent.Add(entity);
                    aspect.LevelLostMarker.Add(entity);
                }
            }
        }
    }
}