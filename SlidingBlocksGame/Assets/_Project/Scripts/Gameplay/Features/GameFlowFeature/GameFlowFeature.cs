using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems;
using _Project.Scripts.Infrastructure;
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
                .AutoDelTag<LevelVictoryEvent>()
                .AutoDelTag<LevelDefeatEvent>()

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
                //.AddUnique(new AnimalCreationChainStrategySystem())
                //.AutoDelTag<CreateAnimalsRequest>()
                //
                .AutoDelTag<MetaGameUIHiddenEvent>()
                .AddUnique(new MetaGameUISystem())
                .AutoDelTag<ShowMetaGameUIRequest>()
                .AutoDelTag<HideMetaGameUIRequest>()
                //
                .AddUnique(new LevelVictoryCheckSystem())
                .AddUnique(new LevelDefeatCheckSystem())
                .AddUnique(new LevelVictorySystem())
                .AddUnique(new LevelDefeatSystem())
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