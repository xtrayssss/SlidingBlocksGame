using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature
{
    public class GameFieldFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddSystem(new RandomGameFieldAlgorithmSystem())
                //
                .AddSystem(new CalculateCellScaleYSystem())
                //
                .AutoDelTag<GameFieldGeneratedEvent>()
                .AutoDelEntityTag<TileGeneratedEvent>()
                .AutoDelTag<GameFieldDestructedEvent>()
                .AddSystem(new GameFieldPlaneAlgorithmSystem())
                .AddSystem(new GameFieldWaveAlgorithmSystem())
                .AddSystem(new GameFieldGrowthWaveAlgorithmSystem())
                //
                .AutoDelTag<GameFieldGenerateRequest>()
                .AutoDelTag<GameFieldDestructRequest>()
                //
                .AutoDelEntityTag<SideClickedEvent>()
                .AddSystem(new GameFieldSideClickSystem())
                .AddSystem(new WithinCenterSystem())
                // audio feature
                .AddSystem(new GameFieldAudioSystem());
        }
    }
}