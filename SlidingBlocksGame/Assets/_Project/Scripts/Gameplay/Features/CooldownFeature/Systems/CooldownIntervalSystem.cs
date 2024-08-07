using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class CooldownIntervalSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Inc] public readonly EcsPool<CooldownInterval> CooldownInterval;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref CooldownInterval cooldownInterval = ref aspect.CooldownInterval.Get(entity);

                cooldownInterval.Elapsed = Mathf.CeilToInt(aspect.Cooldowns.Get(entity).Elapsed / cooldownInterval.Interval) * cooldownInterval.Interval;

                Debug.Log(cooldownInterval.Elapsed);
                Debug.Log(aspect.Cooldowns.Get(entity).Elapsed);
            }
        }
    }
}