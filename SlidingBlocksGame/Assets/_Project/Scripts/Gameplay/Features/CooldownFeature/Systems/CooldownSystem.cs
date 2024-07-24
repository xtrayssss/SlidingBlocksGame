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
            [Inc] public readonly EcsPool<Cooldown> Animations;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref Cooldown cooldown = ref aspect.Animations.Get(entity);

                if ((cooldown.Value += Time.deltaTime) >= cooldown.Duration)
                    _world.GetPool<CooldownExpiredMarker>().Add(entity);
            }
        }
    }
}