using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature
{
    public class InputFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelEntityTag<EmitInputTag>()
                .AddUnique(new InputSystem());
        }
    }
}