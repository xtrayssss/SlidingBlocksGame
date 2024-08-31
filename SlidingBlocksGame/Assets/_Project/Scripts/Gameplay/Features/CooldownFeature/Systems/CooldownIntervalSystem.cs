using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class CooldownIntervalSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Inc] public readonly EcsPool<CooldownInterval> CooldownInterval;
            [Opt] public readonly EcsTagPool<TickEvent> Tick;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref CooldownInterval cooldownInterval = ref aspect.CooldownInterval.Get(entity);

                float last = cooldownInterval.Elapsed;
                
                cooldownInterval.Elapsed =
                    math.ceil(aspect.Cooldowns.Get(entity).Elapsed / cooldownInterval.Interval) *
                    cooldownInterval.Interval;

                if (last != cooldownInterval.Elapsed) 
                    aspect.Tick.Add(entity);
            }
        }
    }
}