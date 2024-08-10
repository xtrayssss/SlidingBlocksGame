using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class BlockMovementChainCommandSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(ChainMovementMarker))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<MovementCommand> MovementCommand;

            [Opt] public readonly EcsTagPool<CalculateDestinationCellRequest> CalculateDestinationCellRequest;
        }

        private class BlockAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
            [Inc] public readonly EcsPool<MovementDirection> Directions;
            //[Exc] public readonly EcsPool<Obstacle> Obstacles;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int blockID))
                {
                    BlockAspect blockAspect = _world.GetAspect<BlockAspect>();

                    if (blockAspect.IsMatches(blockID))
                    {
                        Debug.Log("Movement command: " + blockID);

                        aspect.MovementCommand.Add(blockID);
                        aspect.CalculateDestinationCellRequest.TryAdd(blockID);
                    }
                }
            }
        }
    }
}