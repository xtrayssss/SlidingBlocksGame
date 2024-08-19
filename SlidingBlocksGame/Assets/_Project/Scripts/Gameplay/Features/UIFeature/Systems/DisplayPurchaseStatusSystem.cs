using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class PlayWithSelectedAnimalSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PlayButtonTag> PlayButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }
        private class PurchasedAnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SnappedMarker))]
            [IncImplicit(typeof(PurchasedMarker))]
            [Inc] public readonly EcsPool<SelectionAnimalID> SelectionAnimalIndices;
        }
        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<SelectionAnimalID> SelectionAnimalIndices;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    animalsShopWindowAspect.GameObjectConnects.Get(window).Connect.gameObject.SetActive(false);

                    foreach (int animal in _world.Where(out PurchasedAnimalAspect purchasedAnimalAspect))
                    {
                        foreach (int player in _world.Where(out PlayerAspect playerAspect))
                            playerAspect.SelectionAnimalIndices.Get(player).Value =
                                purchasedAnimalAspect.SelectionAnimalIndices.Read(animal).Value;
                    }
                }
            }
        }
    }

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
            [Inc] public readonly EcsPool<Balance> Balances;
        }

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
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
                        playerAspect.Balances.Get(player).Value -= purchasesAspect.Purchases.Read(purchase).Price;

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