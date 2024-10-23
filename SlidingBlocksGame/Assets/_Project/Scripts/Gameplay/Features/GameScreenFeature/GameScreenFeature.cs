using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature
{
    public class GameScreenFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                //
                .AutoDelTag<GameScreenCreatedEvent>()
                .AddUnique(new CreateGameScreenSystem())
                .AutoDelTag<CreateGameScreenRequest>()
                //
                .AddUnique(new PlayWidgetSystem());
        }
    }
}