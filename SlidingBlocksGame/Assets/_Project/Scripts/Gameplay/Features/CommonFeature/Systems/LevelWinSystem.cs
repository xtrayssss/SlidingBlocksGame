using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class LevelWinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class WinStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelWonEvent))]
            [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            [Opt] public readonly EcsTagPool<CanClickGameFieldMarker> CanClickGameField;
        }

        private class AnimalDestructedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelWonMarker))]
            [IncImplicit(typeof(AnimalDestructedEvent))]
            [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestruct;
        }

        private class GameFieldDestructedEnterStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructedEvent))]
            [Inc] public readonly EcsTagPool<LevelWonMarker> LevelWon;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Opt] public readonly EcsTagPool<ClosedMarker> Closed;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLeveRequest;
        }

        private class GameLossTimerClosedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameLossTimerTag))]
            [IncImplicit(typeof(ClosedMarker))]
            private int _;
        }

        private class LevelClearedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [IncImplicit(typeof(LevelClearedEvent))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [IncImplicit(typeof(LevelClearedEvent))]
            [Inc] public readonly EcsTagPool<LevelWonMarker> LevelWon;

            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out WinStateAspect aspect))
            {
                Debug.Log("WINNER");

                entlong strategy =
                    _world.NewEntityLong(aspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);

                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                aspect.CanClickGameField.Del(level);
            }

            foreach (int level in _world.Where(out AnimalDestructedStateAspect aspect))
                aspect.GameFieldDestruct.Add(level);

            foreach (int _ in _world.Where(out GameFieldDestructedEnterStateAspect _))
            {
                Debug.Log("123");

                foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                {
                    Debug.Log("Disable game loss timer");

                    GameObjectConnect connect = timerAspect.GameObjectConnects.Read(timer);

                    Tween.Scale(connect.Connect.transform, Vector3.zero, 0.2f, Ease.OutQuad)
                        .OnComplete(connect.Connect, target =>
                        {
                            if (!connect.Connect.Entity.TryGetID(out int id))
                                return;

                            timerAspect.Closed.Add(id);
                            target.gameObject.SetActive(false);
                        });
                }
            }

            foreach (int _ in _world.Where(out GameLossTimerClosedAspect _))
            {
                foreach (int game in _world.Where(out GameAspect _))
                    _world.GetPool<CleanupLevelRequest>().Add(game);
            }

            foreach (int game in _world.Where(out LevelClearedStateAspect aspect))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    aspect.NextLevel.Add(game);
                    levelAspect.LevelWon.Del(level);
                }
            }
        }
    }
}