using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature
{
    public class GameFieldFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new RandomGameFieldAlgorithmSystem())
                //
                .AddUnique(new CalculateCellScaleYSystem())
                //
                .AutoDelTag<GameFieldGeneratedEvent>()
                .AutoDelEntityTag<TileGeneratedEvent>()
                .AutoDelTag<GameFieldDestructedEvent>()
                .AddUnique(new GameFieldPlaneAlgorithmSystem())
                .AddUnique(new GameFieldWaveAlgorithmSystem())
                .AddUnique(new GameFieldGrowthWaveAlgorithmSystem())
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