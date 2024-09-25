using _Project.Scripts.Gameplay.Features.AudioFeature;
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
using DCFApixels.DragonECS;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay
{
    public class DestructionFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                //
                .AddUnique(new AnimalDestructionChainStrategySystem())
                .AutoDelTag<AnimalDestructedEvent>()
                .AddUnique(new ChainDestructionRequestSystem())
                .AutoDelTag<ApplyDestructionStrategyRequest>();
        }
    }

    public class EcsRoot : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] public TweenSettings TweenSettings;
        [SerializeField] public TweenSettings<float> TweenSettings2;


        [SerializeField] private ScriptableEntityTemplate _gameCfg;

        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;

        [Button]
        private void ResetProgress()
        {
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
        }

        [Button]
        public void NormalizeMeshSize(GameObject obj, float targetSize = 1f)
        {
            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            
            float lowerY = renderer.bounds.center.y - renderer.bounds.extents.y;
            float upperY = lowerY + renderer.bounds.size.y;

            Debug.Log(renderer.bounds.max);
        }

        [Button]
        public void Stop(GameObject obj, float targetSize = 1f)
        {
            MyStruct1 myStruct1 = new MyStruct1
            {
                value = 2
            };

            MyStruct2 myStruct2 = UnsafeUtility.As<MyStruct1, MyStruct2>(ref myStruct1);

            Debug.Log(myStruct2.value);
        }

        struct MyStruct1
        {
            public int value;
        }

        struct MyStruct2
        {
            public int value;
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
                .AddModule(new VisualFeature())
                .AddModule(new CooldownFeature())
                .AddModule(new AudioFeature())

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