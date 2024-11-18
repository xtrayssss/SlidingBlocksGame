using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature
{
    public class GameFlowFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        private readonly ScriptableEntityTemplate _gameCfg;

        public GameFlowFeature(ScriptableEntityTemplate gameCfg) =>
            _gameCfg = gameCfg;

        protected override void Import(Builder builder)
        {
            builder
                .AddSystem(new CreateGameSystem(_gameCfg))
                //
                .AutoDelTag<LevelChangedEvent>()
                .AddSystem(new NextLevelSystem())
                .AutoDelTag<NextLeveRequest>()
                //
                .AddSystem(new GameFlowSystem())
                .AutoDelTag<GameCreatedEvent>()
                //
                .AddSystem(new MarkLevelCoinDestroyedSystem())
                .AddSystem(new MarkLevelGameOverTimerClosedSystem())
                // 
                .AutoDelTag<LevelVictoryEvent>()
                .AddSystem(new LevelVictoryCheckSystem())
                .AddSystem(new LevelVictorySystem())
                //
                .AutoDelTag<LevelDefeatEvent>()
                .AddSystem(new LevelDefeatCheckSystem())
                .AddSystem(new LevelDefeatSystem())
                //
                .AutoDelTag<LevelClearedEvent>()
                .AddSystem(new CleanupLevelSystem())
                .AutoDelTag<CleanupLevelRequest>();
        }
    }
}