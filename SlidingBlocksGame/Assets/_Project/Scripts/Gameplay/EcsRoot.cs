using _Project.Scripts.Gameplay.Features.AnimalFeature;
using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.CooldownFeature;
using _Project.Scripts.Gameplay.Features.CreationFeature;
using _Project.Scripts.Gameplay.Features.DestructionFeature;
using _Project.Scripts.Gameplay.Features.GameFieldFeature;
using _Project.Scripts.Gameplay.Features.GameFlowFeature;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature;
using DCFApixels.DragonECS;
using Sirenix.OdinInspector;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay
{
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
        
        public void Start()
        {
            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            _pipeline = EcsPipeline.New()
                .AddModule(new GameFlowFeature(_gameCfg))
                .AddModule(new PlayerFeature())
                .AddModule(new CreationFeature())
                .AddModule(new GameFieldFeature(coroutineRunner: this))
                .AddModule(new AnimalFeature())
                .AddModule(new MovementFeature())
                .AddModule(new DestructionFeature())
                .AddModule(new VisualFeature())
                .AddModule(new CooldownFeature())
                .AddModule(new AudioFeature())
                // 
                .AutoDelTag<GameCreatedEvent>()
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