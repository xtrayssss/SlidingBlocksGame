using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature
{
    public class PlayerFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddModule(new InputFeature.InputFeature());
        }
    }
}