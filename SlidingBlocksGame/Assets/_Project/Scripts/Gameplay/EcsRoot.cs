using _Project.Scripts.Gameplay.Features;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Systems;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Systems;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Systems;
using _Project.Scripts.Gameplay.Features.EasingFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Systems;
using DCFApixels.DragonECS;
using Sirenix.OdinInspector;
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

        [Button]
        public void Start()
        {
            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            _pipeline = EcsPipeline.New()

                // creation game world feature
                .AddUnique(new CreateGameSystem(_gameCfg))
                .AddUnique(new CalculateGameFieldSystem())
                .AddUnique(new SelectionGenerationGameFieldSystem())
                .AddUnique(new CalculationScaleGameFieldSystem())
                
                // game field algorithm feature
                .AddUnique(new GameFieldSystem())
                .AddUnique(new WaveGameFieldSystem())
                .AddUnique(new GenerateSmoothnessWaveGameFieldSystem())
                .AutoDelTag<GameFieldDestructedEvent>()
                .AutoDelTag<DestructionGameFieldRequest>()
                
                // create animals feature
                .AddUnique(new CreateAnimalsRequestSystem())
                .AddUnique(new CreateAnimalsSystem())
                .AddUnique(new AnimalCreationChainStrategySystem())
                .AddUnique(new ChainCreationRequestSystem())

                // other
                .AddUnique(new ScaleGameFieldSystem())
                .AddUnique(new HUDSystem())
                .AddUnique(new GameLossTimerSystem())
                .AddUnique(new DetermineClickSystem())
                .AutoDelTag<CreateLevelRequest>()
                .AutoDelTag<CreateAnimalsRequest>()
                .AutoDelTag<GenerateGameFieldRequest>()
                .AutoDelTag<GenerateWaveGameFieldRequest>()
                .AutoDelTag<GenerateSmoothnessWaveGameFieldRequest>()
                .AutoDelTag<GameFieldGeneratedEvent>()
                .AutoDelTag<AnimalPositionedEvent>()

                // easing feature
                .AddUnique(new AnimationCurveSystem())
                .AddUnique(new LinerEasingSystem())

                //occupancy feature
                .AddUnique(new DestinationUnavailabilityCheckRequestSystem())
                .AutoDelTag<DestinationUnavailableMarker>()
                .AddUnique(new DestinationUnavailabilityCheckSystem())
                .AddUnique(new ObstaclePositionAdditionSystem())
                .AddUnique(new NearObstacleCalculationSystem())
                .AddUnique(new DetectionDestinationDistanceSystem())
                .AutoDelTag<DestinationUnavailabilityCheckRequest>()
                .AutoDelTag<DetectionDistanceRequest>()

                // movement feature
                .AddUnique(new TransformSystem())
                .AutoDelTag<UpdateViewRequest>()
                .AddUnique(new WorldPositionSystem())
                .AddUnique(new ChainingBlocksSystem())
                .AddUnique(new ChainMovementSystem())
                .AddUnique(new BlockMovementChainCommandSystem())
                .AddUnique(new CalculateDestinationCellSystem())
                .AddUnique(new MovementEasingCommandSystem())
                .AddUnique(new DestinationMovementSystem())
                .AutoDelTag<CalculateDestinationCellRequest>()

                // end level feature
                .AddUnique(new CellOccupancySystem())

                // destroy feature
                //.AddUnique(new DestroyUnitRequestSystem())
                .AddUnique(new DestroyAnimalSystem())
                
                // destroy feature
                .AddUnique(new LevelLossSystem())

                // chain algorithm
                .AddUnique(new DestructionChainStrategySystem())
                .AddUnique(new ChainDestructionRequestSystem())
                .AutoDelTag<ApplyDestructionStrategyRequest>()
                
                .AddUnique(new CheckAnimalWithinCenterSystem())
                .AddUnique(new LevelWinSystem())
                //.AddUnique(new NextLevelRequestSystem())
                .AddUnique(new NextLevelSystem())
                .AutoDelTag<NextLeveRequest>()
                //.AutoDelTag<LevelWinMarker>()
                
                // visual feature
                .AddUnique(new PlayFxSystem())
                .AutoDelTag<PlayFxRequest>()
                .AddUnique(new DestructionFxSystem())
                .AddUnique(new DestroyFxRequestSystem())
                .AddUnique(new VisualizeGameLossTimerSystem())
                
                // cooldown feature
                .AddUnique(new RefreshCooldownSystem())
                .AddUnique(new DeleteEntityCommandOnExpiredSystem())
                .AddUnique(new CountdownSystem())
                .AddUnique(new CooldownSystem())
                .AddUnique(new CooldownIntervalSystem())
                .AutoDelTag<RefreshCooldownRequest>()

                // other
                .AddUnique(new DestroyViewSystem())
                .AutoDelEntityTag<DeleteEntityCommand>()
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