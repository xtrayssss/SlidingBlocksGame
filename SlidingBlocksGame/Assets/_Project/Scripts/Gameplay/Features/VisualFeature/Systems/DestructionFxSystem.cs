using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Systems
{
    public class DestructionFxSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestroyViewRequest))]
            [Inc] public readonly EcsPool<DestructionFxCfg> DestructionFxs;

            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Opt] public readonly EcsTagPool<PlayFxRequest> PlayFxRequest;
        }

        private class FxAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Prefab> Prefabs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly DestructionFxCfg fxCfg = ref aspect.DestructionFxs.Read(entity);

                entlong fx = _world.NewEntityLong(fxCfg.Value);

                FxAspect fxAspect = _world.GetAspect<FxAspect>();

                if (fxAspect.IsMatches(fx.ID))
                {
                    aspect.PlayFxRequest.Add(fx.ID);

                    EcsEntityConnect connect = Object.Instantiate(fxAspect.Prefabs.Read(fx.ID).Value,
                        aspect.GameObjectConnects.Read(entity).Connect.transform.position, Quaternion.identity);

                    connect.ConnectWith(fx, applyTemplates: false);

                    foreach (MonoEntityTemplateBase monoTemplate in connect.MonoTemplates)
                        monoTemplate.Apply(_world.id, fx.ID);
                }
            }
        }
    }
}