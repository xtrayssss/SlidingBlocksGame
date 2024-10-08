using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.RateUsFeature.Systems
{
    public class RateUsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<RateUsButtonTag> CloseAnimalPurchaseWindowButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _)) 
                YandexGame.ReviewShow(true);
        }
    }
}