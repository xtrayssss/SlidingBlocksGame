using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using ScrollSnap = _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components.ScrollSnap;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class CloseAnimalPurchaseWindowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CloseAnimalsShopWindowButtonTag> CloseAnimalsShopWindowButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [ExcImplicit(typeof(ClosedMarker))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;

            [Inc] public readonly EcsPool<AnimalPurchases> PurchaseAnimals;
            [Inc] public readonly EcsPool<PurchaseButtonStatus> PurchaseButtonStatus;
            [Opt] public readonly EcsTagPool<ClosedStartEvent> ClosedStartEvent;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    ref GameObjectConnect goConnect = ref animalsShopWindowAspect.GameObjectConnects.Get(window);

                    ref ScrollSnap scrollSnap = ref animalsShopWindowAspect.ScrollSnap.Get(window);

                    _world.GetPool<ClosedMarker>().Add(window);

                    scrollSnap.OpenCloseTween.Stop();

                    float factor = 0.7f;
                    
                    animalsShopWindowAspect.ClosedStartEvent.Add(window);
                    
                    scrollSnap.OpenCloseTween = Sequence.Create()
                        .Chain(
                            sequence: AnimatePurchases(
                                animals: animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities,
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
                                target: animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Current.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InBack))
                        // close price
                        .Insert(
                            atTime: delay * factor,
                            tween: Tween.Scale(
                                target: animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Price.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InBack))
                        .ChainCallback(
                            target: goConnect.Connect,
                            callback: static connect =>
                            {
                                connect.gameObject.SetActive(false);

                                if (!connect.Entity.TryGetID(out int id))
                                    return;

                                EcsWorld world = connect.World;
                                
                                world.GetPool<ClosedEvent>().Add(id);

                                ref PurchaseButtonStatus purchaseButtonStatus =
                                    ref world.GetPool<PurchaseButtonStatus>().Get(id);

                                purchaseButtonStatus.Lock.transform.localScale = Vector3.one;
                                purchaseButtonStatus.Play.transform.localScale = Vector3.one;
                                purchaseButtonStatus.Unlock.transform.localScale = Vector3.one;
                            });
                }
            }
        }

        private Sequence AnimatePurchases(EcsGroup animals, out float delay)
        {
            Sequence sequence = Sequence.Create();

            for (int i = 0; i < animals.Count; i++)
            {
                EcsSpan visible = default;

                if (!_world.GetPool<ScrollSnappedMarker>().Has(animals[i]))
                    continue;

                if (i > 0 && i < animals.Count - 1)
                    visible = animals.Slice(i - 1, 3);
                else if (i == 0)
                    visible = animals.Slice(i, 2);
                else if (i == animals.Count - 1)
                    visible = animals.Slice(i - 1, 2);

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

                break;
            }

            delay = 0;

            return sequence;
        }
    }
}