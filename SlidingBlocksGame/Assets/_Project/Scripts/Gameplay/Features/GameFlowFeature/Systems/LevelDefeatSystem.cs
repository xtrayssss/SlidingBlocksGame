using System.Collections.Generic;
using _Project.Scripts.DragonAPI;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Utils;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class LevelDefeatSystem : IEcsInit, IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;
        private DragonCoroutineRunner _coroutineRunner;

        private class LevelDefeatStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelDefeatEvent))]
            [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

            [Inc] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;

            [Opt] public readonly EcsTagPool<CoinDestroyedMarker> CoinDestroyedMarker;
            [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestructRequest;
            [Opt] public readonly EcsPool<GameFieldGeneratedByAlgorithm> GameFieldGeneratedByAlgorithm;
            [Opt] public readonly EcsTagPool<CleanupLevelRequest> CleanupLevel;
            [Opt] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;
            [Opt] public readonly EcsTagPool<GameFieldDestructedMarker> GameFieldDestructedMarker;
            [Opt] public readonly EcsTagPool<AnimalDestructedEvent> AnimalDestructedEvent;
            [Opt] public readonly EcsTagPool<LevelClearedEvent> LevelClearedEvent;
        }

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;

            [Opt] public readonly EcsTagPool<DeleteEntityRequest> DeleteEntity;

            [Opt] public readonly EcsPool<GameObjectConnect> GoConnects;
            [Opt] public readonly EcsTagPool<ViewDestroyedEvent> ViewDestroyedEvent;
            [Opt] public readonly EcsTagPool<DiedEvent> DiedEvent;
        }

        private class CoinDestroyedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<LevelTag> LevelTag;
            [Inc] public readonly EcsTagPool<CoinDestroyedMarker> CoinDestroyedMarker;
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

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Opt] public readonly EcsTagPool<ShowMetaGameUIRequest> ShowMetaGameUI;
        }

        public void Init() =>
            _coroutineRunner = DragonAPI.DragonAPI.CreateCoroutineRunner();

        public void Run()
        {
            _coroutineRunner.Tick();

            foreach (int level in _world.Where(out LevelDefeatStateAspect levelAspect))
            {
#if UNITY_EDITOR
                Debug.Log("LEVEL_DEFEAT");
#endif
                _coroutineRunner.StartCoroutine(HandleLevelDefeatState(levelAspect, level));
            }
        }

        private IEnumerator<CustomYieldInstruction> HandleLevelDefeatState(
            LevelDefeatStateAspect levelAspect, int levelID)
        {
            entlong level = levelID.ToEntityLong(_world);

            InitiateDefeatSequence();

            yield return WaitForAnimalDestruction();

            DestroyCoins();

            yield return WaitForCoinDestruction();

            CleanupGameFieldAndTimer();

            yield return WaitForTimerCloseAndFieldDestruction();

            CleanupLevel();

            yield return WaitForLevelCleared();

            FinalizeLevelDefeat();

            yield break;

            void InitiateDefeatSequence()
            {
                ref readonly DestructionAnimalStrategyCfg strategyCfg =
                    ref levelAspect.DestructionAnimalStrategyConfigs.Read(levelID);

                entlong strategy = _world.NewEntityLong(strategyCfg.Value);

                _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(levelID);
                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.LockGameInputMarker.Add(player);

                foreach (int timer in _world.Where(out GameOverTimerAspect gameOverTimerAspect))
                    gameOverTimerAspect.CooldownLockMarker.Add(timer);
            }

            CustomYieldInstruction WaitForAnimalDestruction()
            {
                return new DragonAPI.YieldInstructions.DragonAPI.WaitUntil<entlong>(
                    target: level,
                    static level =>
                    {
                        EcsWorld world = level.World;

                        LevelDefeatStateAspect levelAspect = world.GetAspect<LevelDefeatStateAspect>();

                        return levelAspect.AnimalDestructedEvent.Has(level.ID);
                    });
            }

            void DestroyCoins()
            {
                foreach (int coin in _world.Where(out CoinAspect coinAspect))
                {
                    if (levelAspect.CoinDestroyedMarker.Has(coin))
                        continue;

                    ref GameObjectConnect goConnect = ref coinAspect.GoConnects.Get(coin);

                    AnimateCoinDestruction(ref goConnect);
                }

                return;

                void AnimateCoinDestruction(ref GameObjectConnect connect)
                {
                    Tween.Scale(
                        target: connect.Connect.transform,
                        endValue: Vector3.zero,
                        duration: 0.4f,
                        ease: Ease.InBack
                    ).OnComplete(
                        target: connect.Connect,
                        onComplete: static connect =>
                        {
                            if (!connect.Entity.TryGetID(out int _))
                                return;

                            EcsWorld world = connect.Entity.World;
                            int catcher = world.NewEntity();
                            CoinCatcherAspect.CoinDestroyAnimationCompletedCatcher catcherAspect =
                                world.GetAspect<CoinCatcherAspect.CoinDestroyAnimationCompletedCatcher>();

                            catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                            catcherAspect.CatchCoinDestroyAnimationCompletedRequest.Add(catcher);
                        });
                }
            }

            CustomYieldInstruction WaitForCoinDestruction()
            {
                return new DragonAPI.YieldInstructions.DragonAPI.WaitUntil<entlong>(
                    target: level,
                    static level =>
                    {
                        EcsWorld world = level.World;
                        CoinDestroyedAspect coinDestroyedAspect = world.GetAspect<CoinDestroyedAspect>();
                        return coinDestroyedAspect.CoinDestroyedMarker.Has(level.ID);
                    });
            }

            void CleanupGameFieldAndTimer()
            {
                levelAspect.CoinDestroyedMarker.Del(levelID);

                foreach (int timer in _world.Where(out GameOverTimerAspect gameOverTimerAspect))
                    gameOverTimerAspect.Close.Add(timer);

                ref readonly GameFieldGeneratedByAlgorithm algorithm =
                    ref levelAspect.GameFieldGeneratedByAlgorithm.Read(levelID);

                if (algorithm.Value.TryGetID(out int algorithmID))
                    levelAspect.GameFieldDestructRequest.Add(algorithmID);
            }

            CustomYieldInstruction WaitForTimerCloseAndFieldDestruction()
            {
                return new DragonAPI.YieldInstructions.DragonAPI.WaitUntil<entlong>(
                    target: level,
                    static level =>
                    {
                        EcsWorld world = level.World;
                        LevelDefeatStateAspect levelAspect = world.GetAspect<LevelDefeatStateAspect>();

                        return levelAspect.GameOverTimerClosedMarker.Has(level.ID) &&
                               levelAspect.GameFieldDestructedMarker.Has(level.ID);
                    });
            }

            void CleanupLevel()
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    ref Levels levels = ref gameAspect.Levels.Get(game);
                    levels.LevelsCount = 0;
                    levels.PackIndex = 0;
                }

                levelAspect.CleanupLevel.Add(levelID);
            }

            CustomYieldInstruction WaitForLevelCleared()
            {
                return new DragonAPI.YieldInstructions.DragonAPI.WaitUntil<entlong>(
                    target: level,
                    static level =>
                    {
                        EcsWorld world = level.World;
                        LevelDefeatStateAspect levelClearedAspect = world.GetAspect<LevelDefeatStateAspect>();
                        return levelClearedAspect.LevelClearedEvent.Has(level.ID);
                    });
            }

            void FinalizeLevelDefeat()
            {
                _world.DelEntity(levelID);

                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                    gameScreenAspect.ShowMetaGameUI.Add(screen);

                PlayLevelExitAudio();
                ResetPlayerScores();

                return;

                void PlayLevelExitAudio()
                {
                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        ref readonly AudioEffectInOnLevelExit effectConfig = ref gameAspect.AudioEffects.Read(game);
                        int effect = _world.NewEntity(effectConfig.Value);
                        _world.GetPool<ApplyAudioEffectRequest>().Add(effect);
                        _world.GetPool<RestartAudioRequest>().Add(effect);
                    }
                }

                void ResetPlayerScores()
                {
                    foreach (int player in _world.Where(out PlayerAspect _))
                    {
                        ScoreUtils.UpdateScore(
                            scorable: player,
                            0,
                            overwrite: true);
                    }
                }
            }
        }
    }
}