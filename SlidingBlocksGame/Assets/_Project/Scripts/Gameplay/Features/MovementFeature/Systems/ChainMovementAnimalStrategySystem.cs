using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class ChainMovementAnimalStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class StrategyAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ApplyStrategyRequest))]
            [Inc] public readonly EcsTagPool<ChainMovementMarker> ChainMovement;

            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
        }

        private class CooldownAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ChainMovementMarker> ChainMovement;
            [Inc] public readonly EcsTagPool<CooldownExpiredEvent> CooldownExpired;
            [Inc] public readonly EcsPool<TargetEntity> Targets;
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
        }

        private class MovableAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CanMoveMarker> CanMove;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Exc] public readonly EcsTagPool<MovingMarker> Moving;

            [Inc] public readonly EcsPool<WorldDestination> WorldDestinations;
            [Inc] public readonly EcsPool<MovementSpeedFactor> MovementSpeedFactors;
        }

        private class TweenCompletedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(TweenCompletedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TweenTargetMovableAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<MovingMarker> Moving;
            [Opt] public readonly EcsTagPool<CellOccupancyMarker> CellOccupancy;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out StrategyAspect chainAspect))
            {
                EcsSpan movables = _world.Where(out MovableAspect movableAspect);

                Debug.Log(movables.Count);

                for (int index = 0; index < movables.Count; index++)
                {
                    int movable = movables[index];

                    int cooldown = _world.NewEntity();

                    CooldownAspect cooldownAspect = _world.GetAspect<CooldownAspect>();

                    cooldownAspect.Cooldowns.Add(cooldown).Duration =
                        chainAspect.Cooldowns.Read(entity).Duration * index;
                    cooldownAspect.Refresh.Add(cooldown);

                    chainAspect.ChainMovement.Add(cooldown);
                    
                    movableAspect.CanMove.Del(movable);

                    _world.GetPool<DeleteOnExpiredMarker>().Add(cooldown);
                    _world.GetPool<TargetEntity>().Add(cooldown).Value = movable.ToEntityLong(_world);
                }

                _world.GetPool<DeleteEntityCommand>().Add(entity);
            }

            foreach (int entity in _world.Where(out CooldownAspect strategyCompletedAspect))
            {
                if (strategyCompletedAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    MovableAspect movableAspect = _world.GetAspect<MovableAspect>();

                    ref GameObjectConnect gameObjectConnect = ref movableAspect.GameObjectConnects.Get(targetID);

                    Tween.PositionAtSpeed(gameObjectConnect.Connect.transform,
                        movableAspect.WorldDestinations.Read(targetID).Value,
                        movableAspect.MovementSpeedFactors.Read(targetID).Value, Ease.OutQuart).OnComplete(
                        gameObjectConnect.Connect,
                        target =>
                        {
                            int callback = _world.NewEntity();
                            _world.GetPool<TweenCompletedEvent>().Add(callback);
                            _world.GetPool<TargetEntity>().Add(callback).Value = target.Entity;
                            _world.GetPool<DeleteEntityCommand>().Add(callback);
                        });

                    movableAspect.Moving.Add(targetID);
                }
            }

            foreach (int entity in _world.Where(out TweenCompletedAspect tweenCompletedAspect))
            {
                if (tweenCompletedAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    TweenTargetMovableAspect targetMovableAspect = _world.GetAspect<TweenTargetMovableAspect>();

                    if (targetMovableAspect.IsMatches(targetID))
                    {
                        targetMovableAspect.Moving.Del(targetID);
                        targetMovableAspect.CellOccupancy.Add(targetID);
                    }
                }
            }
        }
    }
}