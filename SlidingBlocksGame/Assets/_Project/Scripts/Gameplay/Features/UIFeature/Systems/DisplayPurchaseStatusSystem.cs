using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayAnimalPurchaseWindowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<AnimalPurchaseWindowButtonTag> AnimalPurchaseWindowButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<InGamePurchaseAnimals> PurchaseAnimals;
            [Inc] public readonly EcsPool<ScrollSnapRef> ScrollSnap;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    ref var gameObjectConnect = ref animalsShopWindowAspect.GameObjectConnects.Get(window);
                    ref var scrollSnap = ref animalsShopWindowAspect.ScrollSnap.Get(window);

                    scrollSnap.Value.IsOffEffects = true;

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                    Sequence.Create()
                        .Group(Tween.Scale(gameObjectConnect.Connect.transform, Vector3.one, 0.15f, Ease.InOutSine))
                        .Chain(AnimateScrollElements(animalsShopWindowAspect.PurchaseAnimals.Read(window).Value)
                            .ChainCallback(
                                () =>
                                {
                                    _world.GetPool<ScrollSnapRef>().Get(window).Value.IsOffEffects = false;
                                }));

                    gameObjectConnect.Connect.gameObject.SetActive(true);
                }
            }
        }

        Sequence AnimateScrollElements(EcsGroup value)
        {
            Sequence sequence = Sequence.Create();

            for (int i = 0; i < value.Count; i++)
            {
                ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(value[i]);

                gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                sequence.Group(Sequence.Create()
                    .Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.one,
                        duration: 0.5f,
                        Ease.OutBack, startDelay: 0.1f * i)));
            }

            return sequence;
        }
    }

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

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    animalsShopWindowAspect.GameObjectConnects.Get(window).Connect.gameObject.SetActive(false);
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