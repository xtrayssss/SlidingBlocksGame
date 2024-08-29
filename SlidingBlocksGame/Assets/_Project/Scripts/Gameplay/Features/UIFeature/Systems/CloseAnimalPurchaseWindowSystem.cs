using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Gameplay.Features.ScrollFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class CloseAnimalPurchaseWindowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CloseAnimalPurchaseWindowButtonTag> CloseAnimalPurchaseWindowButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [ExcImplicit(typeof(ClosedMarker))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<ScrollSnapRef> ScrollSnap;

            [Inc] public readonly EcsPool<AnimalPurchases> PurchaseAnimals;
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
                    ref ScrollSnapRef scrollSnap = ref animalsShopWindowAspect.ScrollSnap.Get(window);

                    _world.GetPool<ClosedMarker>().Add(window);

                    scrollSnap.Sequence.Stop();

                    // TODO: remove closure allocation

                    // scrollSnap.Value.ScrollRect.enabled = false;
                    //
                    // scrollSnap.Value.IsApplyEffects = false;

                    ScrollSnapRef scrollSnapCopy = scrollSnap;

                    scrollSnap.Sequence = Sequence.Create()
                        .Chain(AnimateScrollElements(animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities))
                        .Insert(0.3f,
                            Tween.Scale(gameObjectConnect.Connect.transform, Vector3.zero, 0.15f, Ease.InOutSine))
                        .Insert(0.2f,
                            Tween.Scale(animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Current.transform,
                                Vector3.zero, 0.15f, Ease.InOutSine))
                        .Insert(0.2f,
                            Tween.Scale(animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Price.transform,
                                Vector3.zero, 0.15f, Ease.InOutSine))
                        .ChainCallback(
                            () => { scrollSnapCopy.Value.gameObject.SetActive(false); });
                }
            }
        }

        private Sequence AnimateScrollElements(EcsGroup value)
        {
            Sequence sequence = Sequence.Create();

            for (int i = 0; i < value.Count; i++)
            {
                EcsSpan visibleAnimals = default;

                if (!_world.GetPool<SnappedMarker>().Has(value[i]))
                    continue;

                if (i > 0 && i < value.Count - 1)
                {
                    visibleAnimals = value.ToSpan().Slice(i - 1, 3);
                }
                else if (i == 0)
                {
                    visibleAnimals = value.ToSpan().Slice(i, 2);
                }
                else if (i == value.Count - 1)
                {
                    visibleAnimals = value.ToSpan().Slice(i - 1, 2);
                }

                for (int index = 0; index < visibleAnimals.Count; index++)
                {
                    int animal = visibleAnimals[index];

                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    sequence.Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.zero,
                        duration: 0.08f, Ease.Linear, startDelay: 0.08f * index));
                }

                EcsGroup buffer = value.Clone();

                buffer.ExceptWith(visibleAnimals);

                for (int index = 0; index < buffer.Count; index++)
                {
                    int animal = buffer[index];

                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;
                }
            }

            return sequence;
        }
    }
}