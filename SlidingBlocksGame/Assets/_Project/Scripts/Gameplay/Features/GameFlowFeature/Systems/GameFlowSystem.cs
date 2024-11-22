using _Project.Scripts.Gameplay.Features.AdFeature.Utils;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Utils;
using _Project.Scripts.Gameplay.Features.SettingsFeature.Components;
using _Project.Scripts.Gameplay.Features.TutorialFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class GameFlowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PlayButtonClickedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<ButtonClickedEvent> _buttonClickedEvents;
            [Inc] private readonly EcsTagPool<PlayButtonTag> _playButtonTags;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Inc] public readonly EcsPool<Levels> Levels;

            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
            [Opt] public readonly EcsPool<MenuAudio> MenuMusics;
            [Opt] public readonly EcsPool<AudioEffectInOnLevelEnter> Effects;
            [Opt] public readonly EcsTagPool<CreateGameOverTimerRequest> CreateGameOverTimer;
            [Opt] public readonly EcsTagPool<CreateSettingsRequest> CreateSettings;
        }

        private class GameCreatedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameCreatedEvent))]
            [Opt] public readonly EcsTagPool<CreateGameScreenRequest> CreateGameScreen;
        }

        private class LevelCreationStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(LevelChangedEvent))]
            [Opt] public readonly EcsTagPool<GameFieldGenerateRequest> GameFieldGenerateRequest;
        }

        private class GeneratedGameFieldStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(GameFieldGeneratedEvent))]
            [Opt] public readonly EcsTagPool<CreateAnimalsRequest> CreateAnimals;
        }

        private class AnimalPositionedStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(AnimalPositionedEvent))]
            [Opt] public readonly EcsTagPool<CreateCoinRequest> CreateCoin;
        }

        private class MetaGameUIHiddenStateAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<MetaGameUIHiddenEvent> _metaGameUIHiddenEvents;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Inc] public readonly EcsPool<GameScreen> GameScreens;
            [Opt] public readonly EcsTagPool<HideMetaGameUIRequest> HideMetaGameUI;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Opt] public readonly EcsPool<Scores> Scores;

            [Opt] public readonly EcsTagPool<LoadProgressRequest> LoadProgressRequest;
            [Opt] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        private class GameScreenCreatedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameScreenCreatedEvent> GameScreenCreatedEvent;
        }

        private class CoinSpawnedStateAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CoinSpawnedEvent> CoinSpawnedEvent;
            [Inc] public readonly EcsTagPool<CoinTag> CoinTag;
        }

        private class GameOverTimerOpenedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameOverTimerTag))]
            [Inc] public readonly EcsTagPool<GameOverTimerOpenedEvent> GameOverTimerOpenedEvent;
        }

        private class SettingsCreatedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<SettingsCreatedEvent> SettingsCreatedEvent;
        }

        private class TutorialWindowAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<TutorialWindowTag> TutorialWindowTag;
            [Opt] public readonly EcsTagPool<OpenTutorialRequest> OpenTutorial;
        }
        
        private class PlayWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<PlayWidget> PlayWidgets;
        }

        public void Run()
        {
            foreach (int game in _world.Where(out GameCreatedAspect aspect))
                aspect.CreateGameScreen.Add(game);

            foreach (int _ in _world.Where(out GameScreenCreatedAspect _))
            {
                if (YandexGame.savesData.CheckFirstSession())
                    foreach (int tutorial in _world.Where(out TutorialWindowAspect tutorialAspect))
                        tutorialAspect.OpenTutorial.Add(tutorial);

                foreach (int game in _world.Where(out GameAspect gameAspect))
                    gameAspect.CreateSettings.Add(game);

                foreach (int game in _world.Where(out GameAspect gameAspect))
                    _world.NewAudioEntity(gameAspect.MenuMusics.Get(game).Value);
            }

            foreach (int _ in _world.Where(out SettingsCreatedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.LoadProgressRequest.Add(player);
            }

            foreach (int _ in _world.Where(out PlayButtonClickedAspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    ref Levels levels = ref gameAspect.Levels.Get(game);

                    levels.Randoms ??= new int[levels.Pack.Length][];

                    for (int i = 0; i < levels.Pack.Length; i++)
                    {
                        levels.Randoms[i] = new int[levels.Pack[i].Levels.Length];

                        for (int j = 0; j < levels.Randoms[i].Length; j++)
                            levels.Randoms[i][j] = j;

                        for (int k = levels.Randoms[i].Length - 1; k > 0; k--)
                        {
                            int randomIndex = Random.Range(0, k + 1);

                            (levels.Randoms[i][k], levels.Randoms[i][randomIndex]) =
                                (levels.Randoms[i][randomIndex], levels.Randoms[i][k]);
                        }

#if DEBUG
                        Debug.Log($"Pack {i} levels after shuffle: {string.Join(", ", levels.Randoms[i])}");
#endif
                    }
                }

                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    gameScreenAspect.HideMetaGameUI.Add(gameScreen);

                    SetInteractablePlayWidget(false);
                }
            }

            foreach (int _ in _world.Where(out MetaGameUIHiddenStateAspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                    gameAspect.NextLevel.Add(game);
                
                AdUtils.ShowAdd();
                
                SetInteractablePlayWidget(true);
            }

            foreach (int level in _world.Where(out LevelCreationStateAspect levelAspect))
            {
                levelAspect.GameFieldGenerateRequest.Add(level);

                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    int effect = _world.NewEntity(gameAspect.Effects.Read(game).Value);

                    _world.GetPool<ApplyAudioEffectRequest>().Add(effect);
                }
            }

            foreach (int level in _world.Where(out GeneratedGameFieldStateAspect aspect))
                aspect.CreateAnimals.Add(level);

            foreach (int level in _world.Where(out AnimalPositionedStateAspect aspect))
                aspect.CreateCoin.Add(level);

            foreach (int _ in _world.Where(out CoinSpawnedStateAspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                    gameAspect.CreateGameOverTimer.Add(game);

                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.LockGameInputMarker.Del(player);
            }

            foreach (int _ in _world.Where(out GameOverTimerOpenedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect _))
                    ScoreUtils.UpdateScore(
                        scorable: player,
                        score: 1);
            }
        }

        private void SetInteractablePlayWidget(bool interactable)
        {
            foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
            {
                if (gameScreenAspect.GameScreens.Read(gameScreen).PlayWidgetConnect.Entity
                    .TryGetID(out int playWidgetID))
                {
                    PlayWidgetAspect playWidgetAspect = _world.GetAspect<PlayWidgetAspect>();

                    ref PlayWidget playWidget = ref playWidgetAspect.PlayWidgets.Get(playWidgetID);
                        
                    playWidget.PlayButton.interactable =interactable ;
                    playWidget.ReplayButton.interactable =interactable ;
                }
            }
        }
    }
}