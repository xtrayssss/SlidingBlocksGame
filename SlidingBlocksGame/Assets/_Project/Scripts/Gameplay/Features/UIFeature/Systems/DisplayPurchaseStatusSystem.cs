using _Project.Scripts.Gameplay.Features.CollectionFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayPurchaseStatusSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(ScrollSnappedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
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
                    ref readonly var coins = ref playerAspect.Coins.Get(player);

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