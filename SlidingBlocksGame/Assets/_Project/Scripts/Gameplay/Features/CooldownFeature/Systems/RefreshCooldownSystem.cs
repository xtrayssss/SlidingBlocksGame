using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class RefreshCooldownSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class CooldownAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RefreshCooldownRequest))]
            [ExcImplicit(typeof(CountdownMarker))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Opt] public readonly EcsTagPool<CooldownExpiredMarker> CooldownExpiredMarker;
        }

        private class CountdownAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RefreshCooldownRequest))]
            [IncImplicit(typeof(CountdownMarker))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Opt] public readonly EcsTagPool<CooldownExpiredMarker> CooldownExpiredMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CooldownAspect aspect))
            {
                ref Cooldown cooldown = ref aspect.Cooldowns.Get(entity);

                cooldown.Elapsed = cooldown.Duration;
                
                if (aspect.CooldownExpiredMarker.Has(entity)) 
                    aspect.CooldownExpiredMarker.Del(entity);
            }

            foreach (int entity in _world.Where(out CountdownAspect aspect))
            {
                ref Cooldown cooldown = ref aspect.Cooldowns.Get(entity);

                cooldown.Elapsed = 0;

                if (aspect.CooldownExpiredMarker.Has(entity)) 
                    aspect.CooldownExpiredMarker.Del(entity);
            }
        }
    }
}