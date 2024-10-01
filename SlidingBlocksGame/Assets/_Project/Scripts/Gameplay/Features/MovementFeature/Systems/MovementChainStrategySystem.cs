using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class MovementChainStrategySystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class CooldownExpiredAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(ChainMovementStrategyTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class MovableAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
            [Inc] public readonly EcsPool<WorldDestination> WorldDestinations;
            [Inc] public readonly EcsPool<MovementSpeedFactor> MovementSpeedFactors;

            [Opt] public readonly EcsTagPool<MovingMarker> MovingMarker;
            [Opt] public readonly EcsTagPool<CellOccupancyMarker> CellOccupancyMarker;
        }

        private class MovementTweenCompletedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovementTweenCompletedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CooldownExpiredAspect strategyCompletedAspect))
            {
                if (!strategyCompletedAspect.Targets.Read(entity).Value.TryGetID(out int movableID))
                    continue;

                MovableAspect movableAspect = _world.GetAspect<MovableAspect>();

                ref GameObjectConnect goConnect = ref movableAspect.GoConnects.Get(movableID);

                Tween
                    .PositionAtSpeed(
                        target: goConnect.Connect.transform,
                        endValue: movableAspect.WorldDestinations.Read(movableID).Value,
                        averageSpeed: movableAspect.MovementSpeedFactors.Read(movableID).Value,
                        ease: Ease.OutQuart)
                    .OnComplete(
                        target: goConnect.Connect,
                        onComplete: static connect =>
                        {
                            if (!connect.Entity.TryGetID(out _))
                                return;

                            EcsWorld world = connect.Entity.World;

                            int catcher = world.NewEntity();

                            MovementTweenCatcherAspect catcherAspect =
                                world.GetAspect<MovementTweenCatcherAspect>();

                            catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                            catcherAspect.CatchMovementTweenRequest.Add(catcher);
                        });

                movableAspect.MovingMarker.Add(movableID);
            }

            foreach (int entity in _world.Where(out MovementTweenCompletedAspect tweenCompletedAspect))
            {
                if (!tweenCompletedAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                    continue;

                MovableAspect movableAspect = _world.GetAspect<MovableAspect>();

                movableAspect.MovingMarker.Del(targetID);
                movableAspect.CellOccupancyMarker.Add(targetID);
            }
        }
    }
}