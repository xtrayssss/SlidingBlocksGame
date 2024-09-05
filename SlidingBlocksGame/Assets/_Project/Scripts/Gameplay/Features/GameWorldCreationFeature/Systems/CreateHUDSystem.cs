using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateHUDSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateHUDRequest))]
            [Inc] public readonly EcsPool<HUDPrefab> HUDPrefabs;

            [Opt] public readonly EcsTagPool<CreateControlsRequest> CreateControls;
        }

        private class ScoreUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateControlsRequest))]
            [Inc] public readonly EcsPool<ScoreUIConnect> ScoreUIConnects;
        }

        private class CoinUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateControlsRequest))]
            [Inc] public readonly EcsPool<CoinUIConnect> CoinUIConnects;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GameAspect aspect))
            {
                EcsEntityConnect connect = Object.Instantiate(aspect.HUDPrefabs.Read(entity).Value);

                entlong hud = _world.NewEntityLong();

                connect.Connect(hud, applyTemplates: true);

                aspect.CreateControls.Add(hud.ID);
            }

            foreach (int entity in _world.Where(out ScoreUIAspect aspect))
            {
                ref readonly ScoreUIConnect connect = ref aspect.ScoreUIConnects.Read(entity);

                entlong bestUI = _world.NewEntityLong();

                connect.Value.Connect(bestUI, applyTemplates: true);
            }

            foreach (int entity in _world.Where(out CoinUIAspect aspect))
            {
                ref readonly CoinUIConnect connect = ref aspect.CoinUIConnects.Read(entity);

                entlong coinUI = _world.NewEntityLong();

                connect.Value.Connect(coinUI, applyTemplates: true);
            }
        }
    }
}