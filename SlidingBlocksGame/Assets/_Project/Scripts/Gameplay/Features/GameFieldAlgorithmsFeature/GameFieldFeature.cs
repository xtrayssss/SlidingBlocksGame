using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature
{
    public class GameFieldFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelTag<TileGeneratedEvent>()
                //
                .AddUnique(new SelectionGenerationGameFieldSystem())
                //
                .AutoDelTag<GameFieldGeneratedEvent>()
                .AutoDelTag<GameFieldDestructedEvent>()
                .AddUnique(new GameFieldPlaneAlgorithmSystem())
                .AddUnique(new GameFieldWaveAlgorithmSystem())
                //
                .AutoDelTag<GameFieldGenerateRequest>()
                .AutoDelTag<GameFieldDestructRequest>();
        }
    }
}