using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature
{
    public class PlayerFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddModule(new InputFeature.InputFeature())
                .AddUnique(new SaveLoadPlayerProgressSystem())
                .AutoDelTag<LoadProgressRequest>();
        }
    }
}