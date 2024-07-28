using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class BlockMovementChainCommandSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(ChainMovementMarker))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<MovementCommand> MovementCommand;
            [Opt] public readonly EcsTagPool<DestinationUnavailabilityCheckRequest> DestinationUnavailabilityCheckRequest;
            [Opt] public readonly EcsTagPool<CalculateDestinationCellRequest> CalculateDestinationCellRequest;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int id))
                {
                    aspect.MovementCommand.Add(id);
                    aspect.CalculateDestinationCellRequest.Add(id);
                    aspect.DestinationUnavailabilityCheckRequest.Add(id);
                }
            }
        }
    }
}