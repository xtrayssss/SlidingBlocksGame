using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
{
    public class PurchaseAnimalSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class PurchaseButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<UnlockButtonTag> UnlockButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class PurchasesClearRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ClearPurchasesRequest> ClearPurchasesRequest;
        }

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseAnimalTag))]
            [IncImplicit(typeof(SnappedMarker))]
            [Exc] public readonly EcsTagPool<PurchasedMarker> PurchasedMarker;

            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<CoinsUpdatedEvent> CoinsUpdated;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
            [Opt] public readonly EcsTagPool<PurchasedEvent> PurchasedEvent;
            [Opt] public readonly EcsPool<PurchasedEntity> PurchasedEntity;
        }

        private class PurchasedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseAnimalTag))]
            [IncImplicit(typeof(SnappedMarker))]
            [Inc] public readonly EcsTagPool<PurchasedMarker> PurchasedMarker;
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
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        ref Coins coins = ref playerAspect.Coins.Get(player);

                        coins.Value -= purchasesAspect.Purchases.Read(purchase).Price;

                        purchasesAspect.PurchasedMarker.Add(purchase);

                        int @event = _world.NewEntity();
                        purchasesAspect.CoinsUpdated.Add(@event);
                        purchasesAspect.TargetEntity.Add(@event).Value = player.ToEntityLong(_world);
                        purchasesAspect.PurchasedEvent.Add(@event);
                        purchasesAspect.PurchasedEntity.Add(@event).Value = purchase.ToEntityLong(_world);
                    }
                }
            }

            foreach (int _ in _world.Where(out PurchasesClearRequestAspect _))
            {
                foreach (int purchase in _world.Where(out PurchasedAspect purchasedAspect)) 
                    purchasedAspect.PurchasedMarker.Del(purchase);
            }
        }
    }
}