using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Systems;
using _Project.Scripts.Gameplay.Features.DestroyFeature.c;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
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
                .AddUnique(new GameScreenSystem())
                .AddUnique(new CreateHUDSystem())
                //
                .AddUnique(new MetaGameUISystem())
                .AutoDelTag<ShowMetaGameUIRequest>()
                .AutoDelTag<HideMetaGameUIRequest>()
                //
                .AddUnique(new LevelWinCheckSystem())
                .AddUnique(new LevelLostCheckSystem())
                .AddUnique(new LevelWinSystem())
                .AddUnique(new LevelLossSystem())
                //
                .AddUnique(new CleanupLevelSystem())
                .AutoDelTag<CleanupLevelRequest>()
                //
                .AddUnique(new NextLevelSystem())
                .AutoDelTag<NextLeveRequest>()
                //
                .AddUnique(new GameFlowSystem())
                //
                .AutoDelTag<GameCreatedEvent>();
        }
    }
}