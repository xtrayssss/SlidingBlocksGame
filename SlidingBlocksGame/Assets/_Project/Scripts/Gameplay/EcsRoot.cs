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
using YG;

namespace _Project.Scripts.Gameplay
{
    public class EcsRoot : MonoBehaviour
    {
        [SerializeField] private ScriptableEntityTemplate _gameCfg;

        private EcsPipelineWrapper _pipeline;
        private EcsDefaultWorld _world;

#if DEBUG
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip _clip;
#endif

#if UNITY_EDITOR
        [Button]
        private void ResetProgress()
        {
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
        }

        [Button]
        private void SetTimeScale(float value)
        {
            Time.timeScale = value;
        }
#endif

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

        public void Update()
        {
#if DEBUG
            if (Input.GetButtonDown("Horizontal")) 
                _source.PlayOneShot(_clip);
#endif

            _pipeline.UpdateRun(_world);
        }

        public void OnDestroy()
        {
            _pipeline.Destroy();
            _pipeline = null;

            _world.Destroy();
            _world = null;
        }

        private class GameFeature : EcsModule
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
                    .AddSubmodule<PlayerFeature<PauseAspect>>()
                    .AddSubmodule<CreationFeature<PauseAspect>>()
                    .AddSubmodule<AnimalFeature<PauseAspect>>()
                    .AddSubmodule<MovementFeature<PauseAspect>>()
                    .AddSubmodule<GameFieldFeature<PauseAspect>>()
                    .AddSubmodule<DestructionFeature<PauseAspect>>()
                    .AddSubmodule<PurchaseFeature<PauseAspect>>()
                    .AddSubmodule<RateUsFeature<PauseAspect>>()
                    .AddSubmodule<CoinFeature<PauseAspect>>()
                    .AddSubmodule<ScoreFeature<PauseAspect>>()
                    .AddSubmodule<GameProgressFeature<PauseAspect>>()
                    .AddSubmodule<RewardFeature<PauseAspect>>()
                    .AddSubmodule<GameOverTimerFeature<PauseAspect>>()
                    .AddSubmodule<SettingsFeature<PauseAspect>>()
                    .AddSubmodule<GameAudioFeature<PauseAspect>>()
                    .AddSubmodule<TutorialFeature<PauseAspect>>()
                    .AddSubmodule<GameScreenFeature<PauseAspect>>()
                    .AddSubmodule<VisualFeature<PauseAspect>>()
                    .AddSubmodule<AdFeature>()
                    .AddSubmodule<PauseFeature>()
                    .AddSubmodule<CooldownFeature<PauseAspect>>()
                    .AddSubmodule<AudioFeature<PauseAspect>>();
            }
        }
    }
}