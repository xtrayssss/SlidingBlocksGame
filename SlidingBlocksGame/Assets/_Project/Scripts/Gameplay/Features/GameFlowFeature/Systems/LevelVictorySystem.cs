using System.Collections.Generic;
using _Project.Scripts.DragonAPI;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class LevelVictorySystem : IEcsInit, IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private DragonCoroutineRunner _coroutineRunner;

        private class LevelVictoryAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelVictoryEvent))]
            [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestructRequest;
            [Opt] public readonly EcsPool<GameFieldGeneratedByAlgorithm> GameFieldGeneratedByAlgorithm;
            [Opt] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;
            [Opt] public readonly EcsTagPool<GameFieldDestructedMarker> GameFieldDestructedMarker;
            [Opt] public readonly EcsTagPool<CleanupLevelRequest> CleanupLevel;
            [Opt] public readonly EcsTagPool<AnimalDestructedEvent> AnimalDestructedEvent;
            [Opt] public readonly EcsTagPool<LevelClearedEvent> LevelClearedEvent;
        }

        private class GameOverTimerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameOverTimerTag))]
            [Opt] public readonly EcsTagPool<CloseGameOverTimerRequest> Close;

            [Opt] public readonly EcsTagPool<CooldownLockMarker> CooldownLockMarker;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Exc] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        public void Init() =>
            _coroutineRunner = DragonAPI.DragonAPI.CreateCoroutineRunner();

        private IEnumerator<CustomYieldInstruction> HandleLevelDefeatState(
            LevelVictoryAspect levelAspect, int levelID)
        {
            entlong level = levelID.ToEntityLong(_world);

            InitiateDefeatSequence();

            yield return WaitForAnimalDestruction();

            CleanupGameFieldAndTimer();

            yield return WaitForTimerCloseAndFieldDestruction();

            CleanupLevel();

            yield return WaitForLevelCleared();

            FinalizeLevelVictory();

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

                        LevelVictoryAspect levelAspect = world.GetAspect<LevelVictoryAspect>();

                        return levelAspect.AnimalDestructedEvent.Has(level.ID);
                    });
            }

            void CleanupGameFieldAndTimer()
            {
                foreach (int timer in _world.Where(out GameOverTimerAspect gameOverTimerAspect))
                    gameOverTimerAspect.Close.Add(timer);

                ref readonly GameFieldGeneratedByAlgorithm algorithm =
                    ref levelAspect.GameFieldGeneratedByAlgorithm.Read(levelID);

                if (algorithm.Value.TryGetID(out int algorithmId))
                    levelAspect.GameFieldDestructRequest.Add(algorithmId);
            }

            CustomYieldInstruction WaitForTimerCloseAndFieldDestruction()
            {
                return new DragonAPI.YieldInstructions.DragonAPI.WaitUntil<entlong>(
                    target: level,
                    static level =>
                    {
                        EcsWorld world = level.World;

                        LevelVictoryAspect levelAspect = world.GetAspect<LevelVictoryAspect>();

                        return levelAspect.GameOverTimerClosedMarker.Has(level.ID) &&
                               levelAspect.GameFieldDestructedMarker.Has(level.ID);
                    });
            }

            void CleanupLevel() =>
                levelAspect.CleanupLevel.Add(levelID);

            CustomYieldInstruction WaitForLevelCleared()
            {
                return new DragonAPI.YieldInstructions.DragonAPI.WaitUntil<entlong>(
                    target: level,
                    static level =>
                    {
                        EcsWorld world = level.World;
                        LevelVictoryAspect levelAspect = world.GetAspect<LevelVictoryAspect>();
                        return levelAspect.LevelClearedEvent.Has(level.ID);
                    });
            }

            void FinalizeLevelVictory()
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                    gameAspect.NextLevel.Add(game);

                _world.DelEntity(levelID);
            }
        }

        public void Run()
        {
            _coroutineRunner.Tick();
            
            foreach (int level in _world.Where(out LevelVictoryAspect levelAspect))
            {
#if UNITY_EDITOR
                Debug.Log("LEVEL_VICTORY");
#endif
                _coroutineRunner.StartCoroutine(HandleLevelDefeatState(levelAspect, level));
            }
        }
    }
}