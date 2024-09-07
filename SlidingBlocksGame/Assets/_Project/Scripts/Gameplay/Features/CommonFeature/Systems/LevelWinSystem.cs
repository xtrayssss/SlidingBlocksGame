using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
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
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;

            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            [Opt] public readonly EcsTagPool<CanClickGameFieldMarker> CanClickGameField;
        }

        private class AnimalDestructedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelWonMarker))]
            [IncImplicit(typeof(AnimalDestructedEvent))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> GameFieldAlgorithmConfigs;
        }

        private class GameFieldDestructedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructedEvent))]
            [Inc] public readonly EcsTagPool<LevelWonMarker> LevelWon;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLeveRequest;
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
            {
                int algorithm = _world.NewEntity(aspect.GameFieldAlgorithmConfigs.Get(level).Value);

                _world.GetPool<GameFieldDestructRequest>().Add(algorithm);
                _world.GetPool<TargetEntity>().Add(algorithm).Value = _world.GetEntityLong(level);

                Debug.Log("Game field destruction request");
            }

            foreach (int level in _world.Where(out GameFieldDestructedStateAspect levelAspect))
            {
                foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                {
                    Debug.Log("Disable game loss timer");

                    GameObjectConnect connect = timerAspect.GameObjectConnects.Read(timer);

                    Tween.Scale(connect.Connect.transform, Vector3.zero, 0.2f, Ease.OutQuad)
                        .OnComplete(connect.Connect, target => { target.gameObject.SetActive(false); });
                }

                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    gameAspect.NextLeveRequest.Add(game);
                    _world.GetPool<CleanupLevelRequest>().Add(game);

                    levelAspect.LevelWon.Del(level);
                };
            }
        }
    }
}