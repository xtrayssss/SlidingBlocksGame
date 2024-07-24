using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.EasingFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeautre.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeautre.Systems
{
    public class DestinationMovementSystem : IEcsFixedRunProcess
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [ExcImplicit(typeof(CooldownExpiredMarker))]
            [Inc] public readonly EcsPool<UnityComponent<Rigidbody>> Rigidbodies;
            [Inc] public readonly EcsPool<EasingDestination> Destinations;
            [Inc] public readonly EcsPool<MovementSpeedFactor> Factors;
        }
        
        private class Aspect2 : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [Inc] public readonly EcsPool<UnityComponent<Rigidbody>> Rigidbodies;
            [Inc] public readonly EcsPool<EasingDestination> Destinations;
            [Inc] public readonly EcsPool<MovementSpeedFactor> Factors;
        }

        public void FixedRun()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("123");
                aspect.Rigidbodies.Get(entity).obj.transform.position =
                    aspect.Destinations.Read(entity).Interpolation;
            }
            
            foreach (int entity in _world.Where(out Aspect2 aspect))
            {
                aspect.Rigidbodies.Get(entity).obj.linearVelocity = float3.zero;
            }
        }
    }
}