using _Project.Scripts.Gameplay.Features.InputFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.InputFeature
{
    public class InputFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new InputSystem());
        }
    }
}