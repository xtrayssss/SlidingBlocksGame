using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.InputFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
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
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;

            [Opt] public readonly EcsPool<MenuAudio> MenuMusics;
            [Opt] public readonly EcsPool<AudioEffectInOnLevelEnter> Effects;
        }

        private class GameCreatedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameCreatedEvent))]
            [Opt] public readonly EcsTagPool<CreateHUDRequest> CreateHud;

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
            [Opt] public readonly EcsTagPool<CreateGameLossTimerRequest> CreateGameLossTimer;

            [Opt] public readonly EcsTagPool<CreateCoinRequest> CreateCoin;
        }

        private class MetaGameUIHiddenStateAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<MetaGameUIHiddenEvent> _metaGameUIHiddenEvents;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
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

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsTagPool<CreateGameLossTimerRequest> CreateGameLossTimerRequest;
        }

        public void Run()
        {
            foreach (int game in _world.Where(out GameCreatedAspect aspect))
            {
                aspect.CreateHud.Add(game);
                aspect.CreateGameScreen.Add(game);
            }

            foreach (int _ in _world.Where(out GameScreenCreatedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.LoadProgressRequest.Add(player);

                foreach (int game in _world.Where(out GameAspect gameAspect))
                    _world.NewAudioEntity(gameAspect.MenuMusics.Get(game).Value);
            }

            foreach (int _ in _world.Where(out PlayButtonClickedAspect _))
            {
                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                    gameScreenAspect.HideMetaGameUI.Add(gameScreen);
            }

            foreach (int _ in _world.Where(out MetaGameUIHiddenStateAspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                    gameAspect.NextLevel.Add(game);
            }

            foreach (int level in _world.Where(out LevelCreationStateAspect levelAspect))
            {
                foreach (int player in _world.Where(out PlayerAspect _))
                    ProgressUtils.UpdateScores(player, 1);

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
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                    levelAspect.CreateGameLossTimerRequest.Add(level);

                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.LockGameInputMarker.Del(player);
            }
        }
    }
}