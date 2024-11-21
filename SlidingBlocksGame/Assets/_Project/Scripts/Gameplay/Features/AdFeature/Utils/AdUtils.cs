using _Project.Scripts.Gameplay.Features.AdFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AdFeature.Utils
{
    public static class AdUtils
    {
        private class ShowAdRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ShowAdRequest> ShowAd;
        }

        public static void ShowAdd()
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();
            int ad = world.NewEntity();
            ShowAdRequestAspect requestAspect = world.GetAspect<ShowAdRequestAspect>();
            requestAspect.ShowAd.Add(ad);
        }
    }
}