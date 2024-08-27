using _Project.Scripts.Gameplay.Features;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Systems;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Systems;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Systems;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class EcsRoot : MonoBehaviour
    {
        [SerializeField] private ScriptableEntityTemplate _gameCfg;
        [SerializeField] private EcsEntityConnect _blockPrefab;

        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;

        public void Start()
        {
            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            _pipeline = EcsPipeline.New()

                // creation game world feature
                .AddModule(new GameFlowFeature(_gameCfg))
                .AddUnique(new CreateAnimalsSystem())
                .AutoDelTag<AnimalPositionedEvent>()
                .AddUnique(new AnimalCreationChainStrategySystem())
                .AddUnique(new ChainCreationRequestSystem())
                .AddUnique(new GameLossTimerSystem())
                .AddUnique(new ScaleGameFieldEnvironmentSystem())
                .AutoDelTag<CreateGameLossTimerRequest>()
                .AutoDelTag<CreateAnimalsRequest>()
                //.AutoDelTag<CreateHUDRequest>()

                // click feature
                .AddUnique(new DetermineClickSystem())

                //occupancy feature
                .AutoDelTag<DestinationUnavailabilityCheckRequest>()

                // movement feature
                .AddUnique(new TransformSystem())
                .AutoDelTag<UpdateViewRequest>()
                .AddUnique(new WorldPositionSystem())
                .AddUnique(new DestinationCellSystem())
                .AddUnique(new ChainMovementAnimalStrategySystem())

                // destroy feature
                //.AddUnique(new DestroyUnitRequestSystem())
                .AddUnique(new DestroyAnimalSystem())

                // destroy feature
                .AutoDelTag<AnimalDestructedEvent>()
              
                // chain algorithm
                .AddUnique(new DestructionChainStrategySystem())
                .AddUnique(new ChainDestructionRequestSystem())
                .AutoDelTag<ApplyDestructionStrategyRequest>()
                .AddUnique(new WithinCenterSystem())
                .AddUnique(new BestVisualizeSystem())

                // visual feature
                .AddUnique(new PlayFxSystem())
                .AutoDelTag<PlayFxRequest>()
                .AddUnique(new DestroyFxRequestSystem())
                .AddUnique(new DestructionFxSystem())
                .AddUnique(new VisualizeGameLossTimerSystem())
                .AddUnique(new SettingsMenuSystem())
                .AddUnique(new AudioButtonsSystem())
                .AddUnique(new InAppPopupSystem())
                .AddUnique(new ScrollSystem())
                .AddUnique(new RotationSystem())
                .AddUnique(new CameraRenderSystem())
                .AutoDelTag<ViewUpdatedEvent>()
                .AddUnique(new DisplayPriceAnimalSystem())
                .AddUnique(new PurchaseAnimalSystem())
                .AddUnique(new DisplayPurchaseStatusSystem())
                .AddUnique(new PlayWithSelectedAnimalSystem())
                .AddUnique(new DisplayAnimalPurchaseWindowSystem())
                .AddUnique(new CloseAnimalPurchaseWindowSystem())
                .AutoDelTag<ScrollStartedEvent>()
                .AutoDelTag<ScrollSnappedEvent>()

                // audio feature
                // .AddUnique(new ButtonAudioRequestSystem())
                // .AddUnique(new AudioSystem())
                // .AddUnique(new TileAudioRequestSystem())
                .AddUnique(new PlayAudioSystem())
                .AutoDelTag<PlayAudioRequest>()

                // other
                // .AddUnique(new ScrollSystem())
                // .AddUnique(new NearestSystem())
                // .AddUnique(new EffectSystem())
                .AddModule(new GameFieldAlgorithmsFeature())
                
                .AutoDelTag<ApplyStrategyRequest>()

                // cooldown feature
                .AddUnique(new RefreshCooldownSystem())
                .AddUnique(new DeleteEntityCommandOnExpiredSystem())
                .AutoDelTag<CooldownExpiredEvent>()
                .AddUnique(new CountdownSystem())
                .AddUnique(new CooldownSystem())
                .AddUnique(new CooldownIntervalSystem())
                .AutoDelTag<RefreshCooldownRequest>()
                .AutoDelEntityTag<ButtonClickedEvent>()
                
                .AutoDelTag<SpawnedEvent>()

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