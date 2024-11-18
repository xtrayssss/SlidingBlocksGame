using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature
{
    public class GameProgressFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddSystem(new SaveLoadPlayerProgressSystem())
                .AddSystem(new ResetPlayerProgressSystem())
                .AutoDelTag<LoadProgressRequest>();
        }
    }
}