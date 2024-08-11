using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class LevelWinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> Obstacles1;
            [Inc] public readonly EcsTagPool<WithinCenterMarker> Obstacles2;
        }

        private class TimerAspect2 : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;

            [Opt] public readonly EcsPool<DestructionStrategy> DestructionStrategy;
            //[Opt] public readonly EcsTagPool<DestructionGameFieldRequest> DestructionGameFieldRequest;
        }

        private class DestructionStrategyAnimalsAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(DestructionStrategyTag))]
            private int _;
        }

        private class DestructionGameFieldAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Inc] public readonly EcsTagPool<GameFieldDestructedEvent> _;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLeveRequest;
        }
        
        // TODO: rework
        private bool init = false;

        public void Run()
        {
            foreach (int entity in _world.Where(out LevelAspect levelAspect))
            {
                if (_world.Where(out Aspect _).Count == 4 && !init)
                {
                    init = true;
                    Debug.Log("WINNER");

                    // TODO: rework

                    entlong strategy =
                        _world.NewEntityLong(levelAspect.DestructionAnimalStrategyConfigs.Read(entity).Value);

                    _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                    levelAspect.DestructionStrategy.Add(entity).Value = strategy;
                }
            }

            foreach (int _ in _world.Where(out DestructionStrategyAnimalsAspect _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    //levelAspect.DestructionGameFieldRequest.Add(level);

                    Debug.Log("Game field destruction request");
                }
            }

            foreach (int _ in _world.Where(out DestructionGameFieldAspect _))
            {
                Debug.Log(1);
                foreach (int timer in _world.Where(out TimerAspect2 timerAspect))
                {
                    Debug.Log("Disable game loss timer");
                    timerAspect.GameObjectConnects.Read(timer).Connect.gameObject.SetActive(false);

                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        init = false;
                        gameAspect.NextLeveRequest.Add(game);
                        _world.GetPool<CleanupLevelRequest>().Add(game);
                    }
                }
            }
        }
    }
}