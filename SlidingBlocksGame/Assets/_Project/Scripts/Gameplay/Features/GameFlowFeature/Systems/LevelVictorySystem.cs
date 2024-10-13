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
    public class LevelVictorySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private struct LevelVictoryStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelVictoryEvent))]
                [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

                [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            }

            public class OnUpdate : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelVictoryMarker))]
                [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

                [Opt] public readonly EcsPool<GameFieldGeneratedByAlgorithm> GameFieldGeneratedByAlgorithm;

                [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
                [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestruct;
                [Opt] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;
                [Opt] public readonly EcsTagPool<CleanupLevelRequest> CleanupLevel;
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

        private struct GameOverTimerClosedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<GameOverTimerTag> GameOverTimerTag;
                [Inc] public readonly EcsTagPool<GameOverTimerClosedEvent> GameOverTimerClosedEvent;
            }
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
        
        private class GameOvertTimerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameOverTimerTag))]
            [Opt] public readonly EcsTagPool<CloseGameOverTimerRequest> Close;

            [Opt] public readonly EcsTagPool<CooldownLockMarker> CooldownLockMarker;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLeveRequest;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Exc] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelVictoryStateAspect.OnUpdate levelAspect))
            {
                foreach (int _ in _world.Where(out LevelVictoryStateAspect.OnEnter aspect))
                {
                    Debug.Log("LEVEL_VICTORY");

                    entlong strategy =
                        _world.NewEntityLong(aspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                    _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);
                    _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                        playerAspect.LockGameInputMarker.Add(player);

                    foreach (int timer in _world.Where(out GameOvertTimerAspect gameOvertTimerAspect))
                        gameOvertTimerAspect.CooldownLockMarker.Add(timer);
                }

                foreach (int _ in _world.Where(out AnimalDestructedStateAspect.OnEnter _))
                {
                    if (levelAspect.GameFieldGeneratedByAlgorithm.Read(level).Value.TryGetID(out int algorithmID))
                        levelAspect.GameFieldDestruct.Add(algorithmID);

                    foreach (int timer in _world.Where(out GameOvertTimerAspect gameOvertTimerAspect))
                        gameOvertTimerAspect.Close.Add(timer);
                }

                if (_world.Where(
                        out LevelVictoryStateAspect.GameFieldDestructedAndGameOverTimerClosedAspect gameFieldDestructedAndGameOverTimerClosedAspect).Count > 0)
                {
                    gameFieldDestructedAndGameOverTimerClosedAspect.GameOverTimerClosedMarker.Del(level);
                    gameFieldDestructedAndGameOverTimerClosedAspect.GameFieldDestructedMarker.Del(level);

                    levelAspect.CleanupLevel.Add(level);
                }

                foreach (int _ in _world.Where(out LevelClearedStateAspect.OnEnter aspect))
                {
                    foreach (int game in _world.Where(out GameAspect _))
                        aspect.NextLevel.Add(game);

                    _world.DelEntity(level);
                }

                foreach (int _ in _world.Where(out GameOverTimerClosedStateAspect.OnEnter _)) 
                    levelAspect.GameOverTimerClosedMarker.Add(level);
            }
        }
    }
}