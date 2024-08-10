using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class LevelLossSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class TimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredEvent> Obstacles;
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class TimerAspect2 : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredMarker> Obstacles;
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;

            [Opt] public readonly EcsPool<DestructionStrategy> DestructionStrategy;
            [Opt] public readonly EcsTagPool<DestructionGameFieldRequest> DestructionGameFieldRequest;
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

        public void Run()
        {
            foreach (int _ in _world.Where(out TimerAspect _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    entlong strategy =
                        _world.NewEntityLong(levelAspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                    _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                    levelAspect.DestructionStrategy.Add(level).Value = strategy;

                    Debug.Log("LOSS");
                }
            }

            foreach (int _ in _world.Where(out DestructionStrategyAnimalsAspect _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    levelAspect.DestructionGameFieldRequest.Add(level);

                    Debug.Log("Game field destruction request");
                }
            }

            foreach (int _ in _world.Where(out DestructionGameFieldAspect _))
            {
                foreach (int timer in _world.Where(out TimerAspect2 timerAspect))
                {
                    Debug.Log("Disable game loss timer");
                    timerAspect.GameObjectConnects.Read(timer).Connect.gameObject.SetActive(false);
                }
            }
        }
    }
}