using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Systems
{
    public class DestructionFxSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestroyViewRequest))]
            [Inc] public readonly EcsPool<DestructionFxPrefab> DestructionFxs;

            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class FxAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PlayFxRequest> PlayFxRequest;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly DestructionFxPrefab fxPrefab = ref aspect.DestructionFxs.Read(entity);

                EcsEntityConnect view = Object.Instantiate(fxPrefab.Value,
                    aspect.GameObjectConnects.Read(entity).Connect.transform.position, Quaternion.identity);

                entlong fx = _world.NewEntityLong();

                view.Connect(fx, applyTemplates: true);

                FxAspect fxAspect = _world.GetAspect<FxAspect>();

                fxAspect.PlayFxRequest.Add(fx.ID);
            }
        }
    }
}