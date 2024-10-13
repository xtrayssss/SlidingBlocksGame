using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature
{
    public class UIFeature : IEcsModule
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public UIFeature(ICoroutineRunner coroutineRunner) => 
            _coroutineRunner = coroutineRunner;

        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new SettingsPopupSystem())
                //
                .AddUnique(new TutorialSystem())
                //
                .AddUnique(new PlayWidgetSystem())
                //
                .AddUnique(new Render3DToUISystem())
                .AutoDel<Render3DToUIRequest>()
                //
                .AddModule(new ScrollSnapFeature.ScrollSnapFeature())
                .AddModule(new ButtonFeature.ButtonFeature());
        }
    }
}