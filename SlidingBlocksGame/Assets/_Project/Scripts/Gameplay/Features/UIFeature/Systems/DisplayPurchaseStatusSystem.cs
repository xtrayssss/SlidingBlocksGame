using _Project.Scripts.Gameplay.Features.CollectFeature;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class PurchaseAnimalSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<UnlockButtonTag> UnlockButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseAnimalTag))]
            [IncImplicit(typeof(SnappedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    foreach (int purchase in _world.Where(out PurchasesAspect purchasesAspect))
                    {
                        playerAspect.Coins.Get(player).Value -= purchasesAspect.Purchases.Read(purchase).Price;

                        purchasesAspect.Purchased.Add(purchase);
                    }
                }
            }
        }
    }

    public class DisplayPurchaseStatusSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseAnimalTag))]
            [IncImplicit(typeof(SnappedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Balances;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<PurchaseButtonStatus> Status;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out PurchasesAspect aspect))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    ref readonly var coins = ref playerAspect.Balances.Get(player);

                    foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                    {
                        if (aspect.Purchased.Has(entity))
                        {
                            animalsShopWindowAspect.Status.Get(window).Current = animalsShopWindowAspect.Status.Read(window).Play;
                            
                            animalsShopWindowAspect.Status.Read(window).Play.SetActive(true);

                            animalsShopWindowAspect.Status.Read(window).Unlock.SetActive(false);
                            animalsShopWindowAspect.Status.Read(window).Lock.SetActive(false);
                        }
                        else
                        {
                            if (coins.Value >= aspect.Purchases.Get(entity).Price)
                            {
                                animalsShopWindowAspect.Status.Get(window).Current = animalsShopWindowAspect.Status.Read(window).Unlock;

                                animalsShopWindowAspect.Status.Read(window).Unlock.SetActive(true);

                                animalsShopWindowAspect.Status.Read(window).Play.SetActive(false);
                                animalsShopWindowAspect.Status.Read(window).Lock.SetActive(false);
                            }
                            else
                            {
                                animalsShopWindowAspect.Status.Get(window).Current = animalsShopWindowAspect.Status.Read(window).Lock;

                                animalsShopWindowAspect.Status.Read(window).Lock.SetActive(true);

                                animalsShopWindowAspect.Status.Read(window).Play.SetActive(false);
                                animalsShopWindowAspect.Status.Read(window).Unlock.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}