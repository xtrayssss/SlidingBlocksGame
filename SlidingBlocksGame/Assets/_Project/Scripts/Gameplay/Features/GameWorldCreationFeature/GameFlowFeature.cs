using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CollectFeature;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Systems;
using _Project.Scripts.Gameplay.Features.DestroyFeature.c;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature
{
    public class GameFlowFeature : IEcsModule
    {
        private readonly ScriptableEntityTemplate _gameCfg;

        public GameFlowFeature(ScriptableEntityTemplate gameCfg) =>
            _gameCfg = gameCfg;

        public void Import(EcsPipeline.Builder builder)
        {
            builder
                //events api
                .AutoDelTag<LevelWonEvent>()
                .AutoDelTag<LevelLostEvent>()

                // core
                .AddUnique(new CreateGameSystem(_gameCfg))
                //
                .AutoDelTag<LevelChangedEvent>()
                .AddUnique(new NextLevelSystem())
                .AutoDelTag<NextLeveRequest>()
                //
                .AddUnique(new GameFlowSystem())
                //
                .AutoDelTag<AnimalsShopWindowCreatedEvent>()
                .AddUnique(new GameScreenSystem())
                .AutoDelTag<CreateGameScreenRequest>()
                //
                .AutoDelTag<ScoreUICreatedEvent>()
                .AddUnique(new CreateHUDSystem())
                .AutoDelTag<CreateHUDRequest>()
                .AutoDelTag<CreateControlsRequest>()
                //
                .AddUnique(new GameLossTimerSystem())
                .AutoDelTag<CreateGameLossTimerRequest>()
                //
                .AutoDelTag<AnimalSpawnedEvent>()
                .AddUnique(new CreateAnimalsSystem())
                .AutoDelTag<AnimalPositionedEvent>()
                .AddUnique(new AnimalCreationChainStrategySystem())
                .AutoDelTag<CreateAnimalsRequest>()
                //
                .AutoDelTag<MetaGameUIHiddenEvent>()
                .AddUnique(new MetaGameUISystem())
                .AutoDelTag<ShowMetaGameUIRequest>()
                .AutoDelTag<HideMetaGameUIRequest>()
                //
                .AddUnique(new LevelWinCheckSystem())
                .AddUnique(new LevelLostCheckSystem())
                .AddUnique(new LevelWinSystem())
                //.AddUnique(new LevelLossSystem())
                //
                .AddUnique(new CreateCoinSystem())
                .AutoDelTag<CreateCoinRequest>()
                //
                .AutoDelTag<LevelClearedEvent>()
                .AddUnique(new CleanupLevelSystem())
                .AutoDelTag<CleanupLevelRequest>()
                //             
                .AutoDelEntityTag<CoinsUpdatedEvent>()
                .AddUnique(new CollectCoinSystem())
                //
                .AutoDelEntityTag<ScoresUpdatedEvent>()
                .AddUnique(new ScoresSystem())
                //
                .AutoDelEntityTag<RewardedEvent>()
                .AddUnique(new RewardSystem())
                //
                .AutoDelEntityTag<PurchasedEvent>()
                .AddUnique(new PurchaseAnimalSystem())
                .AutoDelTag<ClearPurchasesRequest>()
                //
                .AddUnique(new UpdatePlayerProgressSystem());
        }
    }
}