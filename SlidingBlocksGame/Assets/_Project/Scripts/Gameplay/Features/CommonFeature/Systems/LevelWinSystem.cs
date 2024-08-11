using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class LevelWinCheckSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Exc] public readonly EcsTagPool<LevelWonEvent> LevelWonEvent;
            [Exc] public readonly EcsTagPool<LevelWonMarker> LevelWonMarker;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> Obstacles1;
            [Inc] public readonly EcsTagPool<WithinCenterMarker> Obstacles2;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out LevelAspect aspect))
            {
                ref readonly GameField gameField = ref aspect.GameFields.Read(entity);

                if (_world.Where(out AnimalAspect _).Count == gameField.EdgeSize * gameField.EdgeSize)
                {
                    aspect.LevelWonEvent.Add(entity);
                    aspect.LevelWonMarker.Add(entity);
                }
            }
        }
    }

    public class LevelWinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelWinStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelWonEvent))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;

            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
        }

        private class LevelDestructionGameFieldStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructedEvent))]
            [Inc] public readonly EcsTagPool<LevelWonMarker> LevelWon;

            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;
        }

        private class LevelDestructionAnimalsStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelWonMarker))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class DestructionStrategyAnimalsAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(DestructionStrategyTag))]
            private int _;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLeveRequest;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out LevelWinStateAspect levelAspect))
            {
                Debug.Log("WINNER");

                // animal destruction
                entlong strategy =
                    _world.NewEntityLong(levelAspect.DestructionAnimalStrategyConfigs.Read(entity).Value);

                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);
            }

            // TODO: rework
            
            foreach (int _ in _world.Where(out DestructionStrategyAnimalsAspect _))
            {
                foreach (int level in _world.Where(out LevelDestructionAnimalsStateAspect levelAspect))
                {
                    // game field destruction
                    int algorithm = _world.NewEntity(levelAspect.GameFieldAlgorithmConfigs.Get(level).Value);

                    _world.GetPool<GameFieldDestructRequest>().Add(algorithm);
                    _world.GetPool<TargetEntity>().Add(algorithm).Value = _world.GetEntityLong(level);

                    Debug.Log("Game field destruction request");
                }
            }

            foreach (int level in _world.Where(out LevelDestructionGameFieldStateAspect levelAspect))
            {
                foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                {
                    Debug.Log("Disable game loss timer");

                    timerAspect.GameObjectConnects.Read(timer).Connect.gameObject.SetActive(false);

                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        gameAspect.NextLeveRequest.Add(game);
                        _world.GetPool<CleanupLevelRequest>().Add(game);

                        levelAspect.LevelWon.Del(level);
                    }
                }
            }
        }
    }
}