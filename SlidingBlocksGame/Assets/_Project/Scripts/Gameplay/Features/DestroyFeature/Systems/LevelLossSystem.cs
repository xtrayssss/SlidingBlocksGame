using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.c;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class LevelLostCheckSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [ExcImplicit(typeof(LevelWonMarker))]
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Exc] public readonly EcsTagPool<LevelLostEvent> LevelLostEvent;
            [Exc] public readonly EcsTagPool<LevelLostMarker> LevelLostMarker;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredEvent> Obstacles;
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out GameLossTimerAspect _))
            {
                foreach (int entity in _world.Where(out LevelAspect aspect))
                {
                    aspect.LevelLostEvent.Add(entity);
                    aspect.LevelLostMarker.Add(entity);
                }
            }
        }
    }

    public class LevelLossSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelLostStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelLostEvent))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;

            [Inc] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
        }

        private class LevelDestructionAnimalsStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelLostMarker))]
            [IncImplicit(typeof(AnimalDestructedEvent))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;
        }

        private class LevelDestructionGameFieldStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructedEvent))]
            [Inc] public readonly EcsTagPool<LevelLostMarker> LevelLost;

            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLeveRequest;

            [Inc] public readonly EcsPool<LevelCounter> LevelCounters;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelLostStateAspect levelAspect))
            {
                entlong strategy =
                    _world.NewEntityLong(levelAspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);

                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                Debug.Log("LOSS");
            }

            foreach (int level in _world.Where(out LevelDestructionAnimalsStateAspect aspect))
            {
                int algorithm = _world.NewEntity(aspect.GameFieldAlgorithmConfigs.Get(level).Value);

                _world.GetPool<GameFieldDestructRequest>().Add(algorithm);
                _world.GetPool<TargetEntity>().Add(algorithm).Value = _world.GetEntityLong(level);

                Debug.Log("Game field destruction request");
            }

            foreach (int level in _world.Where(out LevelDestructionGameFieldStateAspect levelAspect))
            {
                if (levelAspect.GameScreens.Read(level).Value.TryGetID(out int gameScreenID))
                {
                    Debug.Log("Disable game loss timer");

                    _world.GetPool<GameLossTimerConnect>().Read(gameScreenID).Value.gameObject.SetActive(false);

                    levelAspect.LevelLost.Del(level);

                    foreach (int game in _world.Where(out GameAspect _))
                        _world.GetPool<CleanupLevelRequest>().Add(game);

                    foreach (GameObject ui in _world.GetPool<HideUI>().Read(gameScreenID).Value)
                        ui.SetActive(true);

                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        gameAspect.LevelCounters.Get(game).Value = 0;
                    }
                }
            }
        }
    }
}