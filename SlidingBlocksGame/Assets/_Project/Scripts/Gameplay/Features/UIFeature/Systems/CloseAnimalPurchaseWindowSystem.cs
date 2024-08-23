using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

                    scrollSnap.Value.ScrollRect.enabled = false;

                    scrollSnap.Value.IsApplyEffects = false;

                    ScrollSnapRef scrollSnapCopy = scrollSnap;

                    scrollSnap.Sequence = Sequence.Create()
                        .Chain(AnimateScrollElements(animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities))
                        .Insert(0.3f,
                            Tween.Scale(gameObjectConnect.Connect.transform, Vector3.zero, 0.15f, Ease.InOutSine))
                        .ChainCallback(
                            () => { scrollSnapCopy.Value.gameObject.SetActive(false); });

                    foreach (int ent in _world.Where(out SingleAspect<EcsTagPool<ScrollSnappedMarker>> _))
                    {
                        _world.GetPool<ScrollSnappedMarker>().Del(ent);
                    }

                    foreach (int ent in _world.Where(out SingleAspect<EcsTagPool<ScrollStartedMarker>> _))
                    {
                        _world.GetPool<ScrollStartedMarker>().Del(ent);
                    }
                }
            }
        }

        private Sequence AnimateScrollElements(EcsGroup value)
        {
            Sequence sequence = Sequence.Create();

            for (int i = 0; i < value.Count; i++)
            {
                if (_world.GetPool<ScrollSnappedMarker>().Has(value[i]) && i > 0)
                {
                    EcsSpan afterSnapped = value.ToSpan().Slice(i - 1);
                    EcsSpan beforeSnapped = value.ToSpan().Slice(0, i- 1);

                    for (int index = 0; index < afterSnapped.Count; index++)
                    {
                        var animal = afterSnapped[index];
                        Debug.Log(animal);
                        ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                        sequence.Group(Sequence.Create()
                            .Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.zero,
                                duration: 0.5f,
                                Ease.OutQuint, startDelay: 0.1f * index)));
                    }

                    for (int index = 0; index < beforeSnapped.Count; index++)
                    {
                        var animal = beforeSnapped[index];
                        ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                        sequence.Group(Sequence.Create()
                            .Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.zero,
                                duration: 0.5f,
                                Ease.OutQuint, startDelay: 0.1f * index)));
                    }

                    Debug.Log(afterSnapped.Count + " " + beforeSnapped.Count);
                    Debug.Log(beforeSnapped.Count + " " + afterSnapped.Count);

                    return sequence;
                }
            }

            for (int index = 0; index < value.Count; index++)
            {
                int animal = value[index];

                ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                sequence.Group(Sequence.Create()
                    .Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.zero,
                        duration: 0.5f,
                        Ease.OutQuint, startDelay: 0.1f * index)));
            }

            return sequence;
        }
    }
}