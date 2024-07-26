using _Project.Scripts.Gameplay.Features.EasingFeature.Components;
using DCFApixels.DragonECS;
using Cooldown = _Project.Scripts.Gameplay.Features.CooldownFeature.Components.Cooldown;

namespace _Project.Scripts.Gameplay.Features.EasingFeature.Systems
{
    public class AnimationCurveSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AnimationCurveRef> AnimationCurves;
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Inc] public readonly EcsPool<EasingSpeed> Easings;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly AnimationCurveRef animationCurve = ref aspect.AnimationCurves.Get(entity);
                ref readonly Cooldown cooldown = ref aspect.Cooldowns.Get(entity);

                aspect.Easings.Get(entity).Value = animationCurve.Value.Evaluate(cooldown.Elapsed / cooldown.Duration);
            }
        }
    }
}