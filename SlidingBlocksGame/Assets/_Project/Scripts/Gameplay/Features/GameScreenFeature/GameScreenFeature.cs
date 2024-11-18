using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature
{
    public class GameScreenFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                //
                .AutoDelTag<GameScreenCreatedEvent>()
                .AddSystem(new CreateGameScreenSystem())
                .AutoDelTag<CreateGameScreenRequest>()
                //
                .AddSystem(new PlayWidgetSystem());
        }
    }
}