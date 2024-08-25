using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateHUDSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class HUDAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateHUDRequest))]
            [Inc] public readonly EcsPool<HUDPrefab> HUDPrefabs;
            [Opt] public readonly EcsTagPool<CreateBestRequest> CreateBest;
        }

        private class BestAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateBestRequest))]
            [Inc] public readonly EcsPool<BestConnect> BestConnects;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out HUDAspect aspect))
            {
                EcsEntityConnect connect = Object.Instantiate(aspect.HUDPrefabs.Read(entity).Value);

                entlong hud = _world.NewEntityLong();

                connect.Connect(hud, applyTemplates: true);

                aspect.CreateBest.Add(entity);
            }

            foreach (int entity in _world.Where(out BestAspect aspect))
            {
                ref readonly BestConnect connect = ref aspect.BestConnects.Read(entity);

                entlong best = _world.NewEntityLong();

                connect.Value.Connect(best, applyTemplates: true);
            }
        }
    }
}