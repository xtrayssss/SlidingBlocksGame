using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
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

        private class BestScoreUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateControlsRequest))]
            [Inc] public readonly EcsPool<BestScoreUIConnect> BestScoreUIConnects;
        }

        private class HUDAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CanvasRef> Canvases;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GameAspect aspect))
            {
                EcsEntityConnect connect = Object.Instantiate(aspect.HUDPrefabs.Read(entity).Value);

                entlong hud = _world.NewEntityLong();

                connect.Connect(hud, applyTemplates: true);

                aspect.CreateControls.Add(hud.ID);

                HUDAspect hudAspect = _world.GetAspect<HUDAspect>();

                Camera uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();

                hudAspect.Canvases.Get(hud.ID).Value.worldCamera = uiCamera;
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

            foreach (int entity in _world.Where(out BestScoreUIAspect aspect))
            {
                ref readonly BestScoreUIConnect connect = ref aspect.BestScoreUIConnects.Read(entity);

                entlong bestScoreUI = _world.NewEntityLong();

                connect.Value.Connect(bestScoreUI, applyTemplates: true);
            }
        }
    }
}