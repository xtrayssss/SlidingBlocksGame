using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
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
            [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            [Opt] public readonly EcsTagPool<CanClickGameFieldMarker> CanClickGameField;
        }

        private class AnimalDestructedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelWonMarker))]
            [IncImplicit(typeof(AnimalDestructedEvent))]
            [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestruct;
        }

        private class GameFieldDestructedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructedEvent))]
            [Inc] public readonly EcsTagPool<LevelWonMarker> LevelWon;

            [Opt] public readonly EcsTagPool<GameFieldGenerateRequest> GameFieldGenerate;
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
                aspect.GameFieldDestruct.Add(level);

            foreach (int level in _world.Where(out GameFieldDestructedStateAspect levelAspect))
            {
                foreach (int timer in _world.Where(out GameLossTimerAspect timerAspect))
                {
                    Debug.Log("Disable game loss timer");

                    GameObjectConnect connect = timerAspect.GameObjectConnects.Read(timer);

                    Tween.Scale(connect.Connect.transform, Vector3.zero, 0.2f, Ease.OutQuad)
                        .OnComplete(connect.Connect, target =>
                        {
                            levelAspect.GameFieldGenerate.Add(level);
                            target.gameObject.SetActive(false);
                        });
                }

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