using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature
{
    public class VisualFeature : IEcsModule
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public VisualFeature(ICoroutineRunner coroutineRunner) => 
            _coroutineRunner = coroutineRunner;

        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddModule(new CameraFeature.CameraFeature())
                .AddModule(new VfxFeature())
                .AddModule(new UIFeature.UIFeature(_coroutineRunner));
        }
    }
}