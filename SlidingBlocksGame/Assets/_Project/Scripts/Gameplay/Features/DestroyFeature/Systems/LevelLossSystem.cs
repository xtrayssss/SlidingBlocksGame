using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.c;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class LevelLossSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LossStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelLostEvent))]
                [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

                [Inc] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
                [Opt] public readonly EcsTagPool<CanClickGameFieldMarker> CanClickGameField;
            }
        }

        private class AnimalDestructedStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<LevelLostMarker> LevelLostMarker;
                [Inc] public readonly EcsTagPool<AnimalDestructedEvent> AnimalDestructedEvent;
            }
        }

        private class DestructedGameFieldStateAspect
        {
            public class OnUpdate : EcsAspectAuto
            {
                [ExcImplicit(typeof(LevelClearedEvent))]
                [IncImplicit(typeof(GameFieldDestructedMarker))]
                [Inc] public readonly EcsTagPool<LevelLostMarker> LevelLostMarker;
            }
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Inc] public readonly EcsPool<Levels> Levels;
            [Inc] public readonly EcsPool<AudioEffectInOnLevelExit> AudioEffects;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameLossTimerTag))]
            [Opt] public readonly EcsTagPool<CloseGameLossTimerRequest> Close;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Opt] public readonly EcsTagPool<ShowMetaGameUIRequest> ShowMetaGameUI;
        }

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;

            [Opt] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelLostMarker))]
            [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestruct;
        }

        private class GameLossTimerClosedStateAspect
        {
            public class OnUpdate : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<GameLossTimerTag> GameLossTimerTag;
                [Inc] public readonly EcsTagPool<ClosedMarker> ClosedMarker;
            }
        }

        private class CoinViewDestroyedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<CoinTag> CoinTag;
                [Inc] public readonly EcsTagPool<ViewDestroyedEvent> ViewDestroyedEvent;
            }

            public class OnUpdate : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<CoinDestroyedMarker> CoinDestroyedMarker;
                [Inc] public readonly EcsTagPool<LevelTag> LevelTag;
                [Inc] public readonly EcsTagPool<LevelLostMarker> LevelLostMarker;
            }
        }

        private class LevelClearedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelTag))]
                [IncImplicit(typeof(LevelLostMarker))]
                [IncImplicit(typeof(LevelClearedEvent))]
                [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
            }
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LossStateAspect.OnEnter aspect))
            {
                entlong strategy =
                    _world.NewEntityLong(aspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);

                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                aspect.CanClickGameField.Del(level);

                Debug.Log("LOSS");
            }

            foreach (int level in _world.Where(out AnimalDestructedStateAspect.OnEnter _))
            {
                EcsSpan ecsSpan = _world.Where(out CoinAspect coinAspect);
                
                if (ecsSpan.Count > 0)
                {
                    foreach (int coin in ecsSpan)
                    {
                        ref GameObjectConnect goConnect = ref coinAspect.GameObjectConnects.Get(coin);

                        Tween
                            .Scale(
                                target: goConnect.Connect.transform,
                                endValue: Vector3.zero,
                                duration: 0.4f,
                                ease: Ease.InBack)
                            .OnComplete(
                                target: goConnect.Connect,
                                onComplete: static connect =>
                                {
                                    if (!connect.Entity.TryGetID(out int id))
                                        return;

                                    EcsWorld world = connect.Entity.World;

                                    CoinAspect coinAspect = world.GetAspect<CoinAspect>();

                                    coinAspect.DestroyView.Add(id);
                                });
                    }
                }
                else
                {
                    _world.GetPool<CoinDestroyedMarker>().Add(level);
                }
            }

            foreach (int _ in _world.Where(out CoinViewDestroyedStateAspect.OnEnter _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    Debug.Log("CoinViewDestroyedStateAspect");

                    levelAspect.GameFieldDestruct.Add(level);
                    
                    foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                        timerAspect.Close.Add(timer);
                }
            }

            foreach (int _ in _world.Where(out CoinViewDestroyedStateAspect.OnUpdate _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    Debug.Log("CoinViewDestroyedStateAspect");

                    levelAspect.GameFieldDestruct.Add(level);

                    _world.GetPool<CoinDestroyedMarker>().Del(level);

                    foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                        timerAspect.Close.Add(timer);
                }
            }

            foreach (int level in _world.Where(out DestructedGameFieldStateAspect.OnUpdate _))
            {
                foreach (int _ in _world.Where(out GameLossTimerClosedStateAspect.OnUpdate _))
                {
                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        _world.GetPool<CleanupLevelRequest>().Add(level);
                        gameAspect.Levels.Get(game).LevelIndex = 0;
                    }
                }
            }

            foreach (int level in _world.Where(out LevelClearedStateAspect.OnEnter _))
            {
                Debug.Log("Cleared");

                _world.DelEntity(level);

                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                    gameScreenAspect.ShowMetaGameUI.Add(screen);
                
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    int effect = _world.NewEntity(gameAspect.AudioEffects.Read(game).Value);
                    _world.GetPool<ApplyAudioEffectRequest>().Add(effect);
                    _world.GetPool<RestartAudioRequest>().Add(effect);
                }
            }
        }
    }
}