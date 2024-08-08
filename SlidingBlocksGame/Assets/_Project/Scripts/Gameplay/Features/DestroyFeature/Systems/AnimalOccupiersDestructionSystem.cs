using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class AnimalOccupiersDestructionSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> Obstacles;
            [Opt] public readonly EcsTagPool<DestroyUnitRequest> DestroyUnitRequest;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameLossTimerTag> _;
            [Inc] public readonly EcsTagPool<CooldownExpiredMarker> _1;
        }

        public void Run()
        {
            foreach (var _ in _world.Where(out GameLossTimerAspect _))
            {
                foreach (int entity in _world.Where(out Aspect aspect)) 
                    aspect.DestroyUnitRequest.Add(entity);
            }
        }
    }
}