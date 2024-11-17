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
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
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
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.Gameplay
{
    public class GameFeature : EcsModule
    {
        private readonly ScriptableEntityTemplate _gameCfg;

        public GameFeature(ScriptableEntityTemplate gameCfg) =>
            _gameCfg = gameCfg;

        private class PauseAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameTag> GameTag;
            [Exc] public readonly EcsTagPool<PausedMarker> PausedMarker;
        }

        protected override void Import(Builder builder)
        {
            builder
                .AddSubmodule(new GameFlowFeature<PauseAspect>(_gameCfg))
                .AddSubmodule(new PlayerFeature<PauseAspect>())
                .AddSubmodule(new CreationFeature<PauseAspect>())
                .AddSubmodule(new AnimalFeature<PauseAspect>())
                .AddSubmodule(new MovementFeature<PauseAspect>())
                .AddSubmodule(new GameFieldFeature<PauseAspect>())
                .AddSubmodule(new DestructionFeature<PauseAspect>())
                .AddSubmodule(new PurchaseFeature<PauseAspect>())
                .AddSubmodule(new RateUsFeature<PauseAspect>())
                .AddSubmodule(new CoinFeature<PauseAspect>())
                .AddSubmodule(new ScoreFeature<PauseAspect>())
                .AddSubmodule(new GameProgressFeature<PauseAspect>())
                .AddSubmodule(new RewardFeature<PauseAspect>())
                .AddSubmodule(new GameOverTimerFeature<PauseAspect>())
                .AddSubmodule(new SettingsFeature<PauseAspect>())
                .AddSubmodule(new GameAudioFeature<PauseAspect>())
                .AddSubmodule(new TutorialFeature<PauseAspect>())
                .AddSubmodule(new GameScreenFeature<PauseAspect>())
                .AddSubmodule(new VisualFeature<PauseAspect>())
                .AddSubmodule(new AdFeature())
                .AddSubmodule(new PauseFeature())
                .AddSubmodule(new CooldownFeature<PauseAspect>())
                .AddSubmodule(new AudioFeature<PauseAspect>());
        }
    }

    public class EcsRoot : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private ScriptableEntityTemplate _gameCfg;

        private EcsPipelineWrapper _pipeline;
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

            _pipeline = EcsPipelineWrapper.New()
                .AddRoot(new GameFeature(_gameCfg))
                .AddUnityDebug(_world)
                .Inject(_world)
                .AutoInject()
                .Build();
        }

        public void Update() =>
            _pipeline.UpdateRun(_world);

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