using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature
{
    public class GameFieldAlgorithmsFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                // events
                .AutoDelTag<GameFieldGeneratedEvent>()
                .AutoDelTag<GameFieldDestructedEvent>()
                .AutoDelTag<TileGeneratedEvent>()

                // core
                .AddUnique(new GameFieldPlaneAlgorithmSystem())
                .AddUnique(new GameFieldWaveAlgorithmSystem())
                //.AddUnique(new GenerateSmoothnessWaveGameFieldSystem())

                // requests
                .AutoDelTag<GameFieldGenerateRequest>()
                .AutoDelTag<GameFieldDestructRequest>();
        }
    }
}