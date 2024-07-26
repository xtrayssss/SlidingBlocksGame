using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.EasingFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.EasingFeature.Systems
{
    public class LinerEasingSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [ExcImplicit(typeof(CooldownExpiredMarker))]
            [Inc] public readonly EcsPool<EasingDestination> Destinations;

            [Inc] public readonly EcsPool<EasingSpeed> Speeds;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref EasingDestination destination = ref aspect.Destinations.Get(entity);

                destination.Interpolation = Vector3.LerpUnclamped(destination.Original, destination.Destination,
                    aspect.Speeds.Read(entity).Value);
            }
        }
    }
}