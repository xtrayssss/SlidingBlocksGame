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
            [ExcImplicit(typeof(CooldownLockMarker))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Inc] public readonly EcsPool<CooldownInterval> CooldownIntervals;
            [Opt] public readonly EcsTagPool<CooldownTickEvent> TickEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref CooldownInterval interval = ref aspect.CooldownIntervals.Get(entity);
                float currentElapsed = aspect.Cooldowns.Get(entity).Elapsed;
                float newElapsed = math.ceil(currentElapsed / interval.Interval) * interval.Interval;

                if (math.abs(interval.Elapsed - newElapsed) > math.EPSILON)
                {
                    interval.Elapsed = newElapsed;
                    aspect.TickEvent.Add(entity);
                }
            }
        }
    }
}