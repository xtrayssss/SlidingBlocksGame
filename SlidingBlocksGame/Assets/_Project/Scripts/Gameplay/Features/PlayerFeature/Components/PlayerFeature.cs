using _Project.Scripts.Gameplay.Features.CollectFeature;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature.Components
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