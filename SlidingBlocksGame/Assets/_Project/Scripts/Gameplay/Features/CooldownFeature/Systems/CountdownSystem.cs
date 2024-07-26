using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class CountdownSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [ExcImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(CountdownMarker))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref Cooldown cooldown = ref aspect.Cooldowns.Get(entity);

                if ((cooldown.Elapsed += Time.deltaTime) >= cooldown.Duration)
                    _world.GetTagPool<CooldownExpiredMarker>().Add(entity);
            }
        }
    }
}