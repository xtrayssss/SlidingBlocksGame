using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayPurchaseStatusSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(SnappedState))]
            [Inc] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out PurchasesAspect aspect))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    ref readonly Coins coins = ref playerAspect.Coins.Get(player);

                    ref PurchaseWidget widget = ref aspect.PurchaseWidgets.Get(entity);
                    
                    if (aspect.Purchased.Has(entity))
                    {
                        widget.StatusWidget.Current = widget.StatusWidget.Play;

                        widget.StatusWidget.Play.SetActive(true);
                        widget.StatusWidget.Unlock.SetActive(false);
                        widget.StatusWidget.Lock.SetActive(false);
                    }
                    else
                    {
                        if (coins.Value >= aspect.Purchases.Get(entity).Price)
                        {
                            widget.StatusWidget.Current = widget.StatusWidget.Unlock;

                             widget.StatusWidget.Unlock.SetActive(true);

                             widget.StatusWidget.Play.SetActive(false);
                             widget.StatusWidget.Lock.SetActive(false);
                        }
                        else
                        {
                            widget.StatusWidget.Current = widget.StatusWidget.Lock;

                             widget.StatusWidget.Lock.SetActive(true);
                             widget.StatusWidget.Play.SetActive(false);
                             widget.StatusWidget.Unlock.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}