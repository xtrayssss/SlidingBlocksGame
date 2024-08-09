using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class CooldownSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [ExcImplicit(typeof(CooldownExpiredMarker))]
            [ExcImplicit(typeof(CountdownMarker))]
            [ExcImplicit(typeof(CooldownLockMarker))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref Cooldown cooldown = ref aspect.Cooldowns.Get(entity);

                if ((cooldown.Elapsed -= Time.deltaTime) <= 0f)
                    _world.GetTagPool<CooldownExpiredMarker>().Add(entity);
            }
        }
    }
}