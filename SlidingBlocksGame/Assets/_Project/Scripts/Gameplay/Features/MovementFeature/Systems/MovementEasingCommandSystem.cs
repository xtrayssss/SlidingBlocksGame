using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.EasingFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class MovementEasingCommandSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovementCommand))]
            [Inc] public readonly EcsPool<Destination> Destinations;

            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;

            [Inc] public readonly EcsPool<MovementEasingCfg> Easings;

            [Opt] public readonly EcsTagPool<MovementCommand> Commands;
        }

        private class EasingAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<EasingDestination> Destinations;
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsPool<TargetEntity> Targets;
            [Opt] public readonly EcsTagPool<MovementCommand> MovementCommands;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                int easing = _world.NewEntity(aspect.Easings.Read(entity).Value);

                EasingAspect easingAspect = _world.GetAspect<EasingAspect>();

                ref EasingDestination destination = ref easingAspect.Destinations.Add(easing);

                destination.Destination = aspect.Destinations.Read(entity).Value;
                destination.Original = aspect.WorldPositions.Read(entity).Value;

                easingAspect.Targets.Add(easing).Value = entity.ToEntityLong(aspect.World);
                easingAspect.Refresh.Add(easing);
                easingAspect.MovementCommands.Add(easing);
                
                _world.GetPool<MovementCommand>().Del(entity);
            }
        }
    }
}