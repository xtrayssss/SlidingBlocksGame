using _Project.Scripts.Gameplay.Features.CollectionFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems
{
    public class PurchaseSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class PurchaseButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PurchaseAnimalButtonTag> PurchaseButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }
        
        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(ScrollSnappedMarker))]
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
                foreach (int purchase in _world.Where(out PurchasesAspect purchasesAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect _))
                    {
                        ProgressUtils.UpdateCoins(player, -purchasesAspect.Purchases.Read(purchase).Price);
                        
                        purchasesAspect.PurchasedMarker.Add(purchase);
                        purchasesAspect.PurchasedEvent.Add(purchase);
                    }
                }
            }
        }
    }
}