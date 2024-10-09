using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
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

        private class GameLossTimerExpiredAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredMarker> CooldownExpiredMarker;
            [Inc] public readonly EcsTagPool<GameOverTimerTag> GameLossTimerTag;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> CellOccupancyMarker;
            [Exc] public readonly EcsTagPool<WithinCenterMarker> WithinCenterMarker;
        }

        private class MovingAnimals : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<MovingMarker> MovingMarker;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out GameLossTimerExpiredAspect _))
            {
                if (_world.Where(out MovingAnimals _).Count != 0)
                    continue;

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
                    aspect.LevelLostEvent.Add(entity);
                    aspect.LevelLostMarker.Add(entity);
                }
            }
        }
    }
}