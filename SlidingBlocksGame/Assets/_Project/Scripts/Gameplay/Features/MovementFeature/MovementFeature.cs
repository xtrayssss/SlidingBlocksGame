using _Project.Scripts.Gameplay.Features.CommonFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature
{
    public class MovementFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new GameFieldClickSystem())
                .AddUnique(new DestinationCellSystem())
                .AddUnique(new ChainMovementAnimalStrategySystem())
                .AddUnique(new WithinCenterSystem());
        }
    }
}