using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
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
            [IncImplicit(typeof(GameOverTimerTag))]
            [Inc] public readonly EcsPool<Cooldown> Cooldown;

            [Opt] public readonly EcsTagPool<CloseGameOverTimerRequest> Close;
            [Opt] public readonly EcsTagPool<CooldownLockMarker> CooldownLockMarker;
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
            [Inc] public readonly EcsPool<GameFieldGeneratedByAlgorithm> GameFieldGeneratedByAlgorithm;

            [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestructRequest;
        }

        private class GameLossTimerClosedStateAspect
        {
            public class OnUpdate : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<GameOverTimerTag> GameLossTimerTag;
                [Inc] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;
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

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Opt] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LossStateAspect.OnEnter aspect))
            {
                entlong strategy =
                    _world.NewEntityLong(aspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);

                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.LockGameInputMarker.Add(player);

                foreach (int timer in _world.Where(out GameLossTimerAspect gameLossTimerAspect))
                    gameLossTimerAspect.CooldownLockMarker.Add(timer);

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
                foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                    timerAspect.Close.Add(timer);
                
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    Debug.Log("CoinViewDestroyedStateAspect");

                    if (levelAspect.GameFieldGeneratedByAlgorithm.Read(level).Value.TryGetID(out int algorithmID))
                        levelAspect.GameFieldDestructRequest.Add(algorithmID);
                }
            }

            foreach (int _ in _world.Where(out CoinViewDestroyedStateAspect.OnUpdate _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    Debug.Log("CoinViewDestroyedStateAspect");

                    if (levelAspect.GameFieldGeneratedByAlgorithm.Read(level).Value.TryGetID(out int algorithmID))
                        levelAspect.GameFieldDestructRequest.Add(algorithmID);

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
                        ref Levels levels = ref gameAspect.Levels.Get(game);
                        levels.LevelsCount = 0;
                        levels.PackIndex = 0;
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

                foreach (int player in _world.Where(out PlayerAspect _))
                {
                    ProgressUtils.UpdateScores(
                        target: player,
                        0,
                        overwrite: true);
                }
            }
        }
    }
}