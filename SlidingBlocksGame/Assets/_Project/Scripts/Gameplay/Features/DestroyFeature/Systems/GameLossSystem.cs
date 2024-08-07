using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class GameLossSystem : IEcsRun
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
            [Opt] public readonly EcsTagPool<DestructionStrategyRequest> DestructionStrategyRequest;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out TimerAspect _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    Debug.Log("123");
                    levelAspect.DestructionStrategyRequest.Add(level);
                }
            }
        }
    }
}