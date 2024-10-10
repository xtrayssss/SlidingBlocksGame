using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature
{
    public class GameFieldFeature : IEcsModule
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public GameFieldFeature(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new RandomGameFieldAlgorithmSystem())
                //
                .AddUnique(new CalculateCellScaleYSystem())
                //
                .AutoDelEntityTag<TileGeneratedEvent>()
                .AddUnique(new GameFieldPlaneAlgorithmSystem())
                .AddUnique(new GameFieldWaveAlgorithmSystem(_coroutineRunner))
                .AddUnique(new GameFieldGrowthWaveAlgorithmSystem(_coroutineRunner))
                .AutoDelTag<GameFieldGeneratedEvent>()
                .AutoDelTag<GameFieldDestructedEvent>()
                .AddUnique(new CatchGameFieldEventsSystem())
                //
                .AutoDelTag<GameFieldGenerateRequest>()
                .AutoDelTag<GameFieldDestructRequest>()
                //
                .AutoDelEntityTag<SideClickedEvent>()
                .AddUnique(new GameFieldSideClickSystem())
                .AddUnique(new WithinCenterSystem())
                // audio feature
                .AddUnique(new GameFieldAudioSystem());
        }
    }
}