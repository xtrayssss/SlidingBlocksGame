using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature
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
                .AddUnique(new GameFieldPlaneAlgorithmSystem())
                .AddUnique(new GameFieldWaveAlgorithmSystem(_coroutineRunner))
                .AddUnique(new GameFieldGrowthWaveAlgorithmSystem(_coroutineRunner))
                .AutoDelTag<GameFieldGeneratedEvent>()
                .AutoDelTag<GameFieldDestructedEvent>()
                .AddUnique(new CatchGameFieldEventsSystem())
                //
                .AutoDelTag<GameFieldGenerateRequest>()
                .AutoDelTag<GameFieldDestructRequest>();
        }
    }
}