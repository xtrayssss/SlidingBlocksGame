using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class CountdownSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [ExcImplicit(typeof(CooldownLockMarker))]
            [IncImplicit(typeof(CountdownMarker))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            
            [Exc] public readonly EcsTagPool<CooldownExpiredMarker> CooldownExpiredMarker;
            [Exc] public readonly EcsTagPool<CooldownExpiredEvent> CooldownExpiredEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref Cooldown cooldown = ref aspect.Cooldowns.Get(entity);

                if ((cooldown.Elapsed += Time.deltaTime) >= cooldown.Duration)
                {
                    aspect.CooldownExpiredEvent.Add(entity);
                    aspect.CooldownExpiredMarker.Add(entity);
                }
            }
        }
    }
}