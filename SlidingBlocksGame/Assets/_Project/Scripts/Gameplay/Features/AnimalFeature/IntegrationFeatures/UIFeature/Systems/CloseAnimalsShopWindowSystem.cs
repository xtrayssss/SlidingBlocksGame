using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Utils;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using ScrollSnap = _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components.ScrollSnap;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Systems
{
    public class CloseAnimalsShopWindowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CloseWindowButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CloseAnimalsShopWindowButtonTag> CloseAnimalsShopWindowButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [ExcImplicit(typeof(AnimalsShopWindowClosedMarker))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;

            [Inc] public readonly EcsPool<Purchases> PurchaseAnimals;
            [Inc] public readonly EcsPool<AnimalsShopWindow> AnimalsShopWindows;
            [Opt] public readonly EcsTagPool<LockScrollSnapRequest> LockScrollSnap;
            [Opt] public readonly EcsTagPool<AnimalPurchaseWindowClosedEvent> AnimalPurchaseWindowClosedEvent;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out CloseWindowButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect windowAspect))
                {
                    ref GameObjectConnect goConnect = ref windowAspect.GameObjectConnects.Get(window);

                    ref AnimalsShopWindow animalsShopWindow = ref windowAspect.AnimalsShopWindows.Get(window);
                    ref ScrollSnap scrollSnap = ref windowAspect.ScrollSnap.Get(window);

                    _world.GetPool<AnimalsShopWindowClosedMarker>().Add(window);

                    scrollSnap.OpenCloseTween.Stop();

                    const float factor = 0.7f;

                    windowAspect.LockScrollSnap.Add(window);

                    scrollSnap.OpenCloseTween = Sequence.Create()
                        .Chain(
                            sequence: AnimatePurchases(
                                animals: windowAspect.PurchaseAnimals.Read(window).Entities,
                                in scrollSnap,
                                out float delay))
                        // close window
                        .Insert(
                            atTime: delay,
                            tween: Tween.Scale(
                                target: goConnect.Connect.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InBack))
                        // close purchase button
                        .Insert(
                            atTime: delay * factor,
                            tween: Tween.Scale(
                                target: animalsShopWindow.PurchaseStatusWidget.Current.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InBack))
                        // close price
                        .Insert(
                            atTime: delay * factor,
                            tween: Tween.Scale(
                                target: animalsShopWindow.PurchaseStatusWidget.Price.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InBack))
                        // close window
                        .ChainCallback(
                            target: goConnect.Connect,
                            static connect =>
                            {
                                connect.gameObject.SetActive(false);

                                if (!connect.Entity.TryGetID(out int id))
                                    return;

                                EcsWorld world = connect.World;

                                AnimalsShopWindowAspect windowAspect =
                                    world.GetAspect<AnimalsShopWindowAspect>();

                                windowAspect.AnimalPurchaseWindowClosedEvent.Add(id);

                                ref AnimalsShopWindow animalsShopWindow =
                                    ref windowAspect.AnimalsShopWindows.Get(id);

                                animalsShopWindow.PurchaseStatusWidget.Lock.transform.localScale = Vector3.one;
                                animalsShopWindow.PurchaseStatusWidget.Play.transform.localScale = Vector3.one;
                                animalsShopWindow.PurchaseStatusWidget.Unlock.transform.localScale = Vector3.one;
                            });
                }
            }
        }

        private Sequence AnimatePurchases(EcsGroup animals, in ScrollSnap scrollSnap, out float delay)
        {
            Sequence sequence = Sequence.Create();

            EcsSpan visible = ScrollSnapUtils.GetVisibles(in scrollSnap);

            float visibleDelay = 0.08f;

            delay = 0.08f * visible.Count;

            foreach (int animal in visible)
            {
                ref GameObjectConnect goConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                sequence.Chain(
                    Tween.Scale(
                        target: goConnect.Connect.transform,
                        endValue: Vector3.zero,
                        duration: visibleDelay,
                        ease: Ease.Linear));
            }

            EcsGroup invisible = animals.Clone();

            invisible.ExceptWith(visible);

            //EcsGroup invisible = EcsGroup.Except(animals, visible);

            foreach (int animal in invisible)
            {
                ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                gameObjectConnect.Connect.transform.localScale = Vector3.zero;
            }


            delay = 0;

            return sequence;
        }
    }
}