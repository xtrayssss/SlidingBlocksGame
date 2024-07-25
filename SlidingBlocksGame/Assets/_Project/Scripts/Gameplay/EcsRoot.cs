using _Project.Scripts.Gameplay.Features.CooldownFeature.Systems;
using _Project.Scripts.Gameplay.Features.EasingFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeautre.Systems;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class EcsRoot : MonoBehaviour
    {
        [SerializeField] private ScriptableEntityTemplate _gameCfg;
        [SerializeField] private EcsEntityConnect _blockPrefab;

        public AnimationCurve curve;
        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;

        public void Start()
        {
            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            _pipeline = EcsPipeline.New()

                // creation game world feature
                .AddUnique(new CreateGameSystem(_gameCfg))
                .AddUnique(new CalculateGameFieldSystem())
                .AddUnique(new SelectionGenerationGameFieldSystem())
                .AddUnique(new GenerateGameFieldSystem())
                .AddUnique(new GenerateWaveGameFieldSystem())
                .AddUnique(new GenerateSmoothnessWaveGameFieldSystem())
                .AddUnique(new CreateBlocksRequestingSystem())
                .AddUnique(new CreateBlocksSystem())
                .AddUnique(new DetermineClickSystem())
                
                .AutoDelTag<CreateGameRequest>()
                .AutoDelTag<CreateBlocksRequest>()
                .AutoDelTag<GenerateGameFieldRequest>()
                .AutoDelTag<GenerateWaveGameFieldRequest>()
                .AutoDelTag<GenerateSmoothnessWaveGameFieldRequest>()

                // easing feature
                .AddUnique(new AnimationCurveSystem())
                .AddUnique(new LinerEasingSystem())

                // movement feature
                .AddUnique(new DestinationMovementSystem())

                // cooldown feature
                .AddUnique(new CooldownSystem())
                .AddUnityDebug(_world)
                .Inject(_world)
                .AutoInject()
                .BuildAndInit();
        }

        public void Update() =>
            _pipeline.Run();

        public void FixedUpdate() =>
            _pipeline.FixedRun();

        public void OnDestroy()
        {
            _pipeline.Destroy();
            _pipeline = null;

            _world.Destroy();
            _world = null;
        }
    }
}