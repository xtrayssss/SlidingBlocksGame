using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class RotationSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [IncImplicit(typeof(SnappedMarker))]
            [Inc] public readonly EcsPool<GameObjectConnect> Connects;

            [Inc] public readonly EcsPool<RotationSpeedFactor> Factors;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out AnimalAspect aspect))
            {
                ref GameObjectConnect gameObjectConnect = ref aspect.Connects.Get(entity);
                ref readonly RotationSpeedFactor factor = ref aspect.Factors.Read(entity);

                gameObjectConnect.Connect.transform.Rotate(factor.Value * Vector3.up * Time.deltaTime);
            }
        }
    }
}