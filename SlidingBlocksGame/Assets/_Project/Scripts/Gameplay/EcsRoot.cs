using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.InputFeature;
using _Project.Scripts.Gameplay.Features.MovementFeature;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay
{
    public class AudioFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddAudioSystem<AnimalSpawnedEvent, AnimalSpawnedAudioConfig>()
                .AddAudioSystem<ButtonClickedEvent, ClickedAudioConfig>()
                .AddAudioSystem<DeathEvent, DeathAudioConfig>()
                .AddAudioSystem<TickEvent, TickAudioConfig>()
                .AddUnique(new GameFieldAudioSystem())
                .AddAudioSystem<CoinCollectedEvent, CollectedAudioConfig>()
                .AddAudioSystem<ConfettiExplodedEvent, ConfettiExplodedAudioConfig>()
                .AddAudioSystem<RewardCollectedEvent, RewardCollectedAudioConfig>()
                .AddUnique(new CoinAddedToTextAudioSystem())
                .AddAudioSystem<PurchasedEvent, PurchasedAudioConfig>()
                .AddUnique(new PlayAudioSystem())
                .AutoDelTag<PlayAudioRequest>();
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

    public class EcsRoot : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private ScriptableEntityTemplate _gameCfg;

        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;

        [Button]
        private void ResetProgress()
        {
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
        }

        public Tween tween;
        
        [Button]
        public void NormalizeMeshSize(GameObject obj, float targetSize = 1f)
        {
            tween = Tween.Scale(obj.transform, new Vector3(targetSize, targetSize, targetSize), 3).OnComplete(() =>
            {
                Debug.Log("123");
            });
        }

        [Button]
        public void Stop(GameObject obj, float targetSize = 1f)
        {
            tween.Stop();
        }
        [Button]
        private void Rotate(GameObject go)
        {
            NormalizeMeshSize(go);
        }

        public void Start()
        {
            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            _pipeline = EcsPipeline.New()
                .AutoDelTag<DeathEvent>()
                .AddModule(new GameFlowFeature(_gameCfg))
                .AddModule(new InputFeature())
                .AddModule(new GameFieldFeature(coroutineRunner: this))
                .AddModule(new MovementFeature())
                .AddModule(new DestructionFeature())
                .AddModule(new AudioFeature())
                .AddModule(new VisualFeature())
                .AddModule(new CooldownFeature())

                // 
                .AutoDelTag<GameCreatedEvent>()
                //
                .AutoDelTag<ApplyStrategyRequest>()
                // spawned
                .AutoDelTag<ViewDestroyedEvent>()
                .AddUnique(new DestroyViewSystem())
                .AutoDelEntityTag<DeleteEntityCommand>()
                .AutoDelEntityTag<ButtonClickedEvent>()
                // other
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