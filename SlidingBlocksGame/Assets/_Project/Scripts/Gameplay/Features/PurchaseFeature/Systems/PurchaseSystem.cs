using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Utils;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems
{
    public class PurchaseSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class PurchaseButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PurchaseButtonTag> PurchaseButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
        }
        
        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(SnappedState))]
            [Exc] public readonly EcsTagPool<PurchasedMarker> PurchasedMarker;

            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedEvent> PurchasedEvent;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out PurchaseButtonClickedAspect _))
            {
                foreach (int purchaseID in _world.Where(out PurchasesAspect purchasesAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect _))
                    {
                        ref readonly Purchase purchase = ref purchasesAspect.Purchases.Read(purchaseID);
                        
                        CoinUtils.Update(
                            coinable: player, 
                            coins: -purchase.Price);
                        
                        purchasesAspect.PurchasedMarker.Add(purchaseID);
                        purchasesAspect.PurchasedEvent.Add(purchaseID);
                    }
                }
            }
        }
    }
}