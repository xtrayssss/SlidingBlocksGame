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
            [IncImplicit(typeof(ClosedMarker))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<AnimalPurchases> PurchaseAnimals;
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;
            [Inc] public readonly EcsPool<PurchaseButtonStatus> PurchaseButtonStatus;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    ref GameObjectConnect gameObjectConnect =
                        ref animalsShopWindowAspect.GameObjectConnects.Get(window);

                    _world.GetPool<ClosedMarker>().Del(window);

                    ref ScrollSnap scrollSnap = ref animalsShopWindowAspect.ScrollSnap.Get(window);

                    scrollSnap.OpenCloseTween.Stop();

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                    scrollSnap.OpenCloseTween = Sequence.Create()
                        .Group(Tween.Scale(gameObjectConnect.Connect.transform, Vector3.one, 0.2f, Ease.InOutSine))
                        .Chain(
                            sequence: AnimateScrollElements(
                                value: animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities,
                                purchaseButton: animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Current,
                                price: animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Price,
                                window: window));

                    gameObjectConnect.Connect.transform.gameObject.SetActive(true);
                    
                    EcsDebug.Break();
                }
            }
        }

        private Sequence AnimateScrollElements(EcsGroup value, GameObject purchaseButton,
            GameObject price, int window)
        {
            Sequence sequence = Sequence.Create();

            for (int i = 0; i < value.Count; i++)
            {
                EcsSpan visibleAnimals = default;

                if (!_world.GetPool<SnappedMarker>().Has(value[i]))
                    continue;

                if (i > 0 && i < value.Count - 1)
                {
                    visibleAnimals = value.Slice(i - 1, 3);
                }
                else if (i == 0)
                {
                    visibleAnimals = value.Slice(i, 2);
                }
                else if (i == value.Count - 1)
                {
                    visibleAnimals = value.Slice(i - 1, 2);
                }

                Sequence visibleSequence = Sequence.Create();

                int visibleIndex;

                for (visibleIndex = 0; visibleIndex < visibleAnimals.Count; visibleIndex++)
                {
                    int animal = visibleAnimals[visibleIndex];

                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                    visibleSequence.Group(Tween.Scale(target: gameObjectConnect.Connect.transform,
                        endValue: Vector3.one,
                        duration: 0.08f, Ease.Linear, startDelay: 0.08f * visibleIndex));
                }

                purchaseButton.transform.localScale = Vector3.zero;
                price.transform.localScale = Vector3.zero;

                Tween.Scale(
                    target: purchaseButton.transform,
                    endValue: Vector3.one,
                    duration: 0.08f,
                    ease: Ease.Linear,
                    startDelay: 0.08f * visibleIndex);

                Tween.Scale(
                    target: price.transform,
                    endValue: Vector3.one,
                    duration: 0.08f,
                    ease: Ease.Linear, 
                    startDelay: 0.08f * visibleIndex);

                visibleSequence.ChainCallback(() =>
                {
                    _world.GetPool<ScrollSnap>().Get(window).ScrollRect.enabled = true;
                    _world.GetPool<ApplyEffectsMarker>().Add(window);
                });

                sequence.Group(visibleSequence);

                EcsGroup buffer = value.Clone();

                buffer.ExceptWith(visibleAnimals);

                foreach (int animal in buffer)
                {
                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    gameObjectConnect.Connect.transform.localScale = Vector3.one;
                }

                break;
            }

            return sequence;
        }
    }
}