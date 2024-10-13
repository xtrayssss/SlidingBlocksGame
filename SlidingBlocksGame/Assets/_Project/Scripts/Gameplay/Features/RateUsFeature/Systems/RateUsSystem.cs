using _Project.Scripts.Gameplay.Features.RateUsFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.RateUsFeature.Systems
{
    public class RateUsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<RateUsButtonTag> RateUsButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _)) 
                YandexGame.ReviewShow(authDialog: true);
        }
    }
}