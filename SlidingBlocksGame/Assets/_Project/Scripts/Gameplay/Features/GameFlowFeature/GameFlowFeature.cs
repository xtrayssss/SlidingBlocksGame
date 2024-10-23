using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems;
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
                .AddUnique(new CreateGameSystem(_gameCfg))
                //
                .AutoDelTag<LevelChangedEvent>()
                .AddUnique(new NextLevelSystem())
                .AutoDelTag<NextLeveRequest>()
                //
                .AddUnique(new GameFlowSystem())
                .AutoDelTag<GameCreatedEvent>()
                //
                .AddUnique(new MarkLevelCoinDestroyedSystem())
                .AddUnique(new MarkLevelGameOverTimerClosedSystem())
                // 
                .AutoDelTag<LevelVictoryEvent>()
                .AddUnique(new LevelVictoryCheckSystem())
                .AddUnique(new LevelVictorySystem())
                //
                .AutoDelTag<LevelDefeatEvent>()
                .AddUnique(new LevelDefeatCheckSystem())
                .AddUnique(new LevelDefeatSystem())
                //
                .AutoDelTag<LevelClearedEvent>()
                .AddUnique(new CleanupLevelSystem())
                .AutoDelTag<CleanupLevelRequest>();
        }
    }
}