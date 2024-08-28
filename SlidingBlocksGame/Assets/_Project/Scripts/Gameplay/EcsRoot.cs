using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Systems;
using _Project.Scripts.Gameplay.Features.CooldownFeature;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature;
using _Project.Scripts.Gameplay.Features.MovementFeature;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class AudioFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
        }
    }

    public class DestructionFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                //
                .AddUnique(new DestructionChainStrategySystem())
                .AutoDelTag<AnimalDestructedEvent>()
                .AddUnique(new ChainDestructionRequestSystem())
                .AutoDelTag<ApplyDestructionStrategyRequest>();
        }
    }

    public class EcsRoot : MonoBehaviour
    {
        [SerializeField] private ScriptableEntityTemplate _gameCfg;

        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;

        public void Start()
        {
            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            AudioUtils audioUtils = new AudioUtils();
            
            _pipeline = EcsPipeline.New()

                //
                .AddModule(new GameFlowFeature(_gameCfg))
                .AddModule(new GameFieldFeature())
                .AddModule(new MovementFeature())
                .AddModule(new VisualFeature())
                .AddModule(new DestructionFeature())
                .AddModule(new CooldownFeature())

                // audio feature
                .AddUnique(audioUtils)
                .AddUnique(new ClickedAudioRequestSystem(audioUtils))
                // .AddUnique(new AudioSystem())
                // .AddUnique(new TileAudioRequestSystem())
                .AddUnique(new PlayAudioSystem())
                // .AutoDelTag<PlayAudioRequest>()
                
                // 
                .AutoDelTag<ApplyStrategyRequest>()
                .AutoDelEntityTag<ButtonClickedEvent>()
                // spawned

                // other
                .AddUnique(new DestroyViewSystem())
                .AutoDelEntityTag<DeleteEntityCommand>()
                //
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