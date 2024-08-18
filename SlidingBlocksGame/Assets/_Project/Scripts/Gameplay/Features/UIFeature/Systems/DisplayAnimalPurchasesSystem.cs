using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayAnimalPurchasesSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [IncImplicit(typeof(SnappedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Balance> Balances;
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
                    ref readonly Balance balance = ref playerAspect.Balances.Get(player);

                    foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                    {
                        if (aspect.Purchased.Has(entity))
                        {
                            animalsShopWindowAspect.Status.Read(window).Play.SetActive(true);

                            animalsShopWindowAspect.Status.Read(window).Unlock.SetActive(false);
                            animalsShopWindowAspect.Status.Read(window).Lock.SetActive(false);
                        }
                        else
                        {
                            if (balance.Value >= aspect.Purchases.Get(entity).Price)
                            {
                                animalsShopWindowAspect.Status.Read(window).Unlock.SetActive(true);

                                animalsShopWindowAspect.Status.Read(window).Play.SetActive(false);
                                animalsShopWindowAspect.Status.Read(window).Lock.SetActive(false);
                            }
                            else
                            {
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