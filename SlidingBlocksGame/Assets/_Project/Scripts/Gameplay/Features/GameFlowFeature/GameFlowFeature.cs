using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CollectionFeature.Components;
using _Project.Scripts.Gameplay.Features.CollectionFeature.Systems;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems;
using _Project.Scripts.Gameplay.Features.PurchaseFeature;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Systems;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature
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
                .AutoDelTag<GameScreenCreatedEvent>()
                .AddUnique(new GameScreenSystem())
                .AutoDelTag<CreateGameScreenRequest>()
                //
                .AutoDelTag<ScoreUICreatedEvent>()
                .AddUnique(new CreateHUDSystem())
                .AutoDelTag<CreateHUDRequest>()
                .AutoDelTag<CreateControlsRequest>()
                //
                .AutoDelTag<GameLossTimerOpenedEvent>()
                .AddUnique(new GameLossTimerSystem())
                .AutoDelTag<CreateGameLossTimerRequest>()
                //
                //.AddUnique(new AnimalCreationChainStrategySystem())
                //.AutoDelTag<CreateAnimalsRequest>()
                //
                .AutoDelTag<MetaGameUIHiddenEvent>()
                .AddUnique(new MetaGameUISystem())
                .AutoDelTag<ShowMetaGameUIRequest>()
                .AutoDelTag<HideMetaGameUIRequest>()
                //
                .AddUnique(new LevelWinCheckSystem())
                .AddUnique(new LevelLostCheckSystem())
                .AddUnique(new LevelWinSystem())
                .AddUnique(new LevelLossSystem())
                //
                .AutoDelTag<CoinSpawnedEvent>()
                .AddUnique(new CatchCoinSystem())
                .AddUnique(new CreateCoinSystem())
                .AutoDelTag<CreateCoinRequest>()
                //
                .AutoDelTag<LevelClearedEvent>()
                .AddUnique(new CleanupLevelSystem())
                .AutoDelTag<CleanupLevelRequest>()
                //             
                .AutoDelTag<CoinCollectedEvent>()
                .AddUnique(new CollectCoinSystem())
                //
                .AutoDelEntityComponent<CoinsUpdatedEvent>()
                .AddUnique(new CoinsSystem())
                .AutoDelEntityComponent<UpdateCoinsRequest>()
                //
                .AutoDelEntityTag<RewardUpdatedEvent>()
                .AddUnique(new RewardSystem())
                .AutoDelEntityComponent<UpdateRewardRequest>();
        }
    }
}