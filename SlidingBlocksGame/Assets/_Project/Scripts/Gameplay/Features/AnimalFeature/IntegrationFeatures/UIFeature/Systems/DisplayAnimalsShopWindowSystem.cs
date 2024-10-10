using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayAnimalsShopWindowSystem : IEcsRun
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
            [IncImplicit(typeof(AnimalsShopWindowClosedMarker))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<AnimalsShopWindow> AnimalsShopWindows;
            [Inc] public readonly EcsPool<Purchases> PurchaseAnimals;
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;

            [Opt] public readonly EcsTagPool<UnlockScrollSnapRequest> UnlockScrollSnap;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    ref GameObjectConnect goConnect =
                        ref animalsShopWindowAspect.GameObjectConnects.Get(window);

                    ref AnimalsShopWindow animalsShopWindow =
                        ref animalsShopWindowAspect.AnimalsShopWindows.Get(window);

                    _world.GetPool<AnimalsShopWindowClosedMarker>().Del(window);

                    ref ScrollSnap scrollSnap = ref animalsShopWindowAspect.ScrollSnap.Get(window);

                    scrollSnap.OpenCloseTween.Stop();

                    goConnect.Connect.transform.localScale = Vector3.zero;

                    scrollSnap.OpenCloseTween = Sequence.Create()
                        .Group(
                            Tween.Scale(
                                target: goConnect.Connect.transform,
                                endValue: Vector3.one,
                                duration: 0.2f,
                                ease: Ease.OutBack))
                        .Chain(
                            AnimatePurchases(
                                animals: animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities,
                                purchaseButton: animalsShopWindow.PurchaseStatusWidget.Current,
                                price: animalsShopWindow.PurchaseStatusWidget.Price,
                                in scrollSnap))
                        .ChainCallback(
                            target: goConnect.Connect,
                            static connect =>
                            {
                                if (!connect.Entity.TryGetID(out int id))
                                    return;

                                EcsWorld world = connect.Entity.World;

                                AnimalsShopWindowAspect windowAspect = world.GetAspect<AnimalsShopWindowAspect>();

                                windowAspect.UnlockScrollSnap.Add(id);
                            });

                    goConnect.Connect.transform.gameObject.SetActive(true);
                }
            }
        }

        private Sequence AnimatePurchases(EcsGroup animals, GameObject purchaseButton, GameObject price,
            in ScrollSnap scrollSnap)
        {
            Sequence sequence = Sequence.Create();

            EcsSpan visible = default;

            Debug.Log("AnimatePurchases");

            int i = scrollSnap.TargetIndex;

            if (i > 0 && i < animals.Count - 1)
                visible = animals.Slice(i - 1, 3);
            else if (i == 0)
                visible = animals.Slice(0, 2);
            else if (i == animals.Count - 1)
                visible = animals.Slice(i - 1, 2);

            foreach (int animal in visible)
            {
                ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                sequence.Chain(
                    Tween.Scale(
                        target: gameObjectConnect.Connect.transform,
                        endValue: Vector3.one,
                        duration: 0.2f,
                        ease: Ease.Linear));
            }

            Debug.Log(visible.Count);
            sequence.ChainCallback(() => Debug.Log(123));

            price.transform.localScale = Vector3.zero;
            purchaseButton.transform.localScale = Vector3.zero;

            sequence
                .Chain(
                    Tween.Scale(
                        target: purchaseButton.transform,
                        endValue: Vector3.one,
                        duration: 0.08f,
                        ease: Ease.OutBack))
                .Group(
                    Tween.Scale(
                        target: price.transform,
                        endValue: Vector3.one,
                        duration: 0.08f,
                        ease: Ease.OutBack));

            EcsGroup buffer = animals.Clone();

            buffer.ExceptWith(visible);

            foreach (int animal in buffer)
            {
                ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                gameObjectConnect.Connect.transform.localScale = Vector3.one;
            }

            return sequence;
        }
    }
}