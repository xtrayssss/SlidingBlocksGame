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

        private class LostStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelLostEvent))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;

            [Inc] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            [Opt] public readonly EcsTagPool<CanClickGameFieldMarker> CanClickGameField;
        }

        private class AnimalDestructedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelLostMarker))]
            [IncImplicit(typeof(AnimalDestructedEvent))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;
        }

        private class GameFieldDestructedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructedEvent))]
            [Inc] public readonly EcsTagPool<LevelLostMarker> LevelLost;

            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Inc] public readonly EcsPool<Levels> Levels;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Opt] public readonly EcsTagPool<ShowMetaGameUIRequest> ShowMetaGameUI;
        }

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntity;

            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;
            [Opt] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LostStateAspect aspect))
            {
                entlong strategy =
                    _world.NewEntityLong(aspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);

                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                aspect.CanClickGameField.Del(level);

                Debug.Log("LOSS");
            }

            foreach (int level in _world.Where(out AnimalDestructedStateAspect aspect))
            {
                int algorithm = _world.NewEntity(aspect.GameFieldAlgorithmConfigs.Get(level).Value);

                _world.GetPool<GameFieldDestructRequest>().Add(algorithm);
                _world.GetPool<TargetEntity>().Add(algorithm).Value = _world.GetEntityLong(level);

                foreach (int coin in _world.Where(out CoinAspect coinAspect))
                {
                    ref var gameObjectConnect = ref coinAspect.GameObjectConnects.Get(coin);

                    Tween
                        .Scale(gameObjectConnect.Connect.transform, Vector3.zero, 0.4f, Ease.InBack)
                        .OnComplete(
                            target: gameObjectConnect.Connect,
                            connect =>
                            {
                                if (!connect.Entity.TryGetID(out int coinID))
                                    return;
                                
                                coinAspect.DeleteEntity.Add(coinID);
                                coinAspect.DestroyView.Add(coinID);
                            });
                }

                Debug.Log("Game field destruction request");
            }

            foreach (int level in _world.Where(out GameFieldDestructedStateAspect levelAspect))
            {
                Debug.Log("Disable game loss timer");

                foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                {
                    GameObjectConnect connect = timerAspect.GameObjectConnects.Read(timer);

                    Tween.Scale(connect.Connect.transform, Vector3.zero, 0.2f, Ease.OutQuad)
                        .OnComplete(connect.Connect, target => { target.gameObject.SetActive(false); });

                    levelAspect.LevelLost.Del(level);
                }

                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    _world.GetPool<CleanupLevelRequest>().Add(game);
                    gameAspect.Levels.Get(game).LevelIndex = 0;
                }

                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                    gameScreenAspect.ShowMetaGameUI.Add(gameScreen);
            }
        }
    }
}