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
            [Inc] public readonly EcsTagPool<CooldownExpiredMarker> Obstacles;
            [Inc] public readonly EcsTagPool<GameLossTimerTag> Obstacles1;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;

            [Opt] public readonly EcsPool<DestructionStrategy> DestructionStrategy;
            [Opt] public readonly EcsTagPool<DestructionGameFieldRequest> DestructionGameFieldRequest;
        }

        private class DestructionStrategyAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(DestructionStrategyTag))]
            private int _;
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

            foreach (int s in _world.Where(out DestructionStrategyAspect _))
            {
                Debug.Log(s);
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    levelAspect.DestructionGameFieldRequest.Add(level);
                    
                    Debug.Log("Game field destruction request");
                }
            }
        }
    }
}