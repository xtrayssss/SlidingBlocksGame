using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class LevelDefeatSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private struct LevelDefeatStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelDefeatEvent))]
                [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

                [Inc] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            }

            public class OnUpdate : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelDefeatMarker))]
                [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

                [Inc] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;

                [Opt] public readonly EcsPool<GameFieldGeneratedByAlgorithm> GameFieldGeneratedByAlgorithm;
                [Opt] public readonly EcsTagPool<CoinDestroyedMarker> CoinDestroyedMarker;
                [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestructRequest;
                [Opt] public readonly EcsTagPool<CleanupLevelRequest> CleanupLevel;
                [Opt] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;
            }

            public class GameFieldDestructedAndGameOverTimerClosedAspect : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelTag))]
                [Inc] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;

                [Inc] public readonly EcsTagPool<GameFieldDestructedMarker> GameFieldDestructedMarker;
            }
        }

        private struct AnimalDestructedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<AnimalDestructedEvent> AnimalDestructedEvent;
            }
        }

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;

            [Opt] public readonly EcsTagPool<DeleteEntityRequest> DeleteEntity;

            [Opt] public readonly EcsPool<GameObjectConnect> GoConnects;
            [Opt] public readonly EcsTagPool<ViewDestroyedEvent> ViewDestroyedEvent;
        }

        private class CoinDestroyedStateAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<LevelTag> LevelTag;
            [Inc] public readonly EcsTagPool<CoinDestroyedMarker> CoinDestroyedMarker;
        }

        private struct LevelClearedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelTag))]
                [IncImplicit(typeof(LevelClearedEvent))]
                [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
            }
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Opt] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Inc] public readonly EcsPool<Levels> Levels;

            [Inc] public readonly EcsPool<AudioEffectInOnLevelExit> AudioEffects;
        }

        private class GameOverTimerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameOverTimerTag))]
            [Inc] public readonly EcsPool<Cooldown> Cooldown;

            [Opt] public readonly EcsTagPool<CloseGameOverTimerRequest> Close;
            [Opt] public readonly EcsTagPool<CooldownLockMarker> CooldownLockMarker;
        }
        
        private struct GameOverTimerClosedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<GameOverTimerTag> GameOverTimerTag;
                [Inc] public readonly EcsTagPool<GameOverTimerClosedEvent> GameOverTimerClosedEvent;
            }
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Opt] public readonly EcsTagPool<ShowMetaGameUIRequest> ShowMetaGameUI;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelDefeatStateAspect.OnUpdate levelAspect))
            {
                foreach (int _ in _world.Where(out LevelDefeatStateAspect.OnEnter _))
                {
                    Debug.Log("LEVEL_DEFEAT");

                    entlong strategy =
                        _world.NewEntityLong(levelAspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                    _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);

                    _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                        playerAspect.LockGameInputMarker.Add(player);

                    foreach (int timer in _world.Where(out GameOverTimerAspect gameOverTimerAspect))
                        gameOverTimerAspect.CooldownLockMarker.Add(timer);
                }

                foreach (int _ in _world.Where(out AnimalDestructedStateAspect.OnEnter _))
                {
                    foreach (int coin in _world.Where(out CoinAspect coinAspect))
                    {
                        if (coinAspect.GoConnects.Has(coin))
                        {
                            ref GameObjectConnect goConnect = ref coinAspect.GoConnects.Get(coin);

                            Tween.Scale(
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
                        else
                        {
                            levelAspect.CoinDestroyedMarker.Add(level);
                        }
                    }
                }

                foreach (int _ in _world.Where(out CoinDestroyedStateAspect _))
                {
                    levelAspect.CoinDestroyedMarker.Del(level);

                    foreach (int timer in _world.Where(out GameOverTimerAspect gameOverTimerAspect))
                        gameOverTimerAspect.Close.Add(timer);

                    ref readonly GameFieldGeneratedByAlgorithm algorithm =
                        ref levelAspect.GameFieldGeneratedByAlgorithm.Read(level);

                    if (algorithm.Value.TryGetID(out int algorithmID))
                        levelAspect.GameFieldDestructRequest.Add(algorithmID);
                }

                if (_world.Where(
                        out LevelDefeatStateAspect.GameFieldDestructedAndGameOverTimerClosedAspect _).Count > 0)
                {
                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        ref Levels levels = ref gameAspect.Levels.Get(game);

                        levels.LevelsCount = 0;
                        levels.PackIndex = 0;
                    }

                    levelAspect.CleanupLevel.Add(level);
                }

                foreach (int _ in _world.Where(out LevelClearedStateAspect.OnEnter _))
                {
                    _world.DelEntity(level);

                    foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                        gameScreenAspect.ShowMetaGameUI.Add(screen);

                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        ref readonly AudioEffectInOnLevelExit effectCfg = ref gameAspect.AudioEffects.Read(game);
                        int effect = _world.NewEntity(effectCfg.Value);
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

                foreach (int coin in _world.Where(out CoinAspect coinAspect))
                {
                    if (coinAspect.ViewDestroyedEvent.Has(coin)) 
                        levelAspect.CoinDestroyedMarker.Add(level);
                }
                
                foreach (int _ in _world.Where(out GameOverTimerClosedStateAspect.OnEnter _)) 
                    levelAspect.GameOverTimerClosedMarker.Add(level);
            }
        }
    }
}