using _Project.Scripts.Gameplay.Features.AdFeature;
using _Project.Scripts.Gameplay.Features.AnimalFeature;
using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.CoinFeature;
using _Project.Scripts.Gameplay.Features.CooldownFeature;
using _Project.Scripts.Gameplay.Features.CreationFeature;
using _Project.Scripts.Gameplay.Features.DestructionFeature;
using _Project.Scripts.Gameplay.Features.GameAudioFeature;
using _Project.Scripts.Gameplay.Features.GameFieldFeature;
using _Project.Scripts.Gameplay.Features.GameFlowFeature;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature;
using _Project.Scripts.Gameplay.Features.GameProgressFeature;
using _Project.Scripts.Gameplay.Features.GameScreenFeature;
using _Project.Scripts.Gameplay.Features.MovementFeature;
using _Project.Scripts.Gameplay.Features.PauseFeature;
using _Project.Scripts.Gameplay.Features.PauseFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature;
using _Project.Scripts.Gameplay.Features.PurchaseFeature;
using _Project.Scripts.Gameplay.Features.RateUsFeature;
using _Project.Scripts.Gameplay.Features.RewardFeature;
using _Project.Scripts.Gameplay.Features.ScoreFeature;
using _Project.Scripts.Gameplay.Features.SettingsFeature;
using _Project.Scripts.Gameplay.Features.TutorialFeature;
using _Project.Scripts.Gameplay.Features.VisualFeature;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.Gameplay
{
    
    interface IDoSomethingProcess : IEcsProcess
    {
        void Do();
    }
    // Реализация раннера. Пример реализации можно так же посмотреть в встроенных процессах 
    sealed class DoSomethingProcessRunner : EcsRunner<IDoSomethingProcess>, IDoSomethingProcess
    {
        public void Do() 
        {
            foreach (var item in Process) item.Do();
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

        public void Start()
        {
            YandexGame.ErrorFullAdEvent += () => Debug.Log("Cancelled");
            YandexGame.CloseFullAdEvent += () => Debug.Log("Closed");

            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            _pipeline = EcsPipeline.New()
                .AddModule(new GameFlowFeature(_gameCfg))
                .AddModule(new PlayerFeature())
                .AddModule(new CreationFeature())
                .AddModule(new AnimalFeature())
                .AddModule(new MovementFeature())
                .AddModule(new GameFieldFeature())
                .AddModule(new DestructionFeature())
                .AddModule(new PurchaseFeature())
                .AddModule(new RateUsFeature())
                .AddModule(new CoinFeature())
                .AddModule(new ScoreFeature())
                .AddModule(new GameProgressFeature())
                .AddModule(new RewardFeature())
                .AddModule(new GameOverTimerFeature())
                .AddModule(new SettingsFeature())
                .AddModule(new GameAudioFeature())
                .AddModule(new TutorialFeature())
                .AddModule(new GameScreenFeature())
                .AddModule(new VisualFeature())
                .AddModule(new AdFeature())
                .AddModule(new PauseFeature())
                .AddModule(new CooldownFeature())
                .AddModule(new AudioFeature())
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

        [Button]
        private int GetMaxVisibleElements(RectTransform viewPort, RectTransform elementTemplate, float visiblePartRatio,
            HorizontalLayoutGroup layoutGroup)
        {
            RectTransform viewport = viewPort;
            float viewportWidth = viewport.rect.width;
            float elementWidth = elementTemplate.rect.width;

            // Учитываем padding с обеих сторон
            float totalPadding = layoutGroup.padding.left + layoutGroup.padding.right;
            float availableWidth = viewportWidth - totalPadding;

            // Учитываем spacing между элементами
            float spacing = layoutGroup.spacing;
            float elementWithSpacing = elementWidth + spacing;

            // Вычисляем сколько целых элементов помещается в доступное пространство
            float maxElements = availableWidth / elementWithSpacing;

            return Mathf.FloorToInt(maxElements);
        }
    }
}