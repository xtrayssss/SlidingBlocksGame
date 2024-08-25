using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
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
        }

        private class LevelCreationStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(SpawnedEvent))]
            [Opt] public readonly EcsTagPool<GameFieldGenerateRequest> GameFieldGenerate;
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
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Opt] public readonly EcsTagPool<HideMetaGameUIRequest> HideMetaGameUI;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out PlayButtonClickedAspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                    gameAspect.NextLevel.Add(game);
            }

            foreach (int level in _world.Where(out LevelCreationStateAspect aspect))
                aspect.GameFieldGenerate.Add(level);

            foreach (int level in _world.Where(out GeneratedGameFieldStateAspect aspect))
                aspect.CreateAnimals.Add(level);

            foreach (int level in _world.Where(out AnimalPositionedStateAspect aspect))
            {
                aspect.CreateGameLossTimer.Add(level);

                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect)) 
                    gameScreenAspect.HideMetaGameUI.Add(gameScreen);
            }
        }
    }
}