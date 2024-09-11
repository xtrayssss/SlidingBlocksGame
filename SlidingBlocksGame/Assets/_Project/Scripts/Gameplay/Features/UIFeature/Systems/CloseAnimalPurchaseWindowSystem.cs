using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using ScrollSnap = _Project.Scripts.Gameplay.Features.UIFeature.Components.ScrollSnap;

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
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    ref GameObjectConnect gameObjectConnect =
                        ref animalsShopWindowAspect.GameObjectConnects.Get(window);

                    ref ScrollSnap scrollSnap = ref animalsShopWindowAspect.ScrollSnap.Get(window);

                    _world.GetPool<ClosedMarker>().Add(window);

                    scrollSnap.OpenCloseTween.Stop();

                    scrollSnap.ScrollRect.enabled = false;

                    _world.GetPool<ApplyEffectsMarker>().Del(window);

                    scrollSnap.OpenCloseTween = Sequence.Create()
                        .Chain(
                            sequence: AnimateScrollElements(
                                value: animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities))
                        .Insert(
                            atTime: 0.3f,
                            tween: Tween.Scale(
                                target: gameObjectConnect.Connect.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InBack))
                        .Insert(
                            atTime: 0.2f,
                            tween: Tween.Scale(
                                target: animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Current.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InBack))
                        .Insert(
                            atTime: 0.2f,
                            tween: Tween.Scale(
                                target: animalsShopWindowAspect.PurchaseButtonStatus.Get(window).Price.transform,
                                endValue: Vector3.zero,
                                duration: 0.15f,
                                ease: Ease.InOutSine))
                        .ChainCallback(
                            target: gameObjectConnect.Connect,
                            callback: Callback);
                }
            }
        }

        private static void Callback(EcsEntityConnect connect)
        {
            connect.gameObject.SetActive(false);

            if (!connect.Entity.TryGetID(out int id)) 
                return;

            EcsWorld world = connect.World;

            ref PurchaseButtonStatus purchaseButtonStatus = ref world.GetPool<PurchaseButtonStatus>().Get(id);
            
            purchaseButtonStatus.Lock.transform.localScale = Vector3.one;
            purchaseButtonStatus.Play.transform.localScale = Vector3.one;
            purchaseButtonStatus.Unlock.transform.localScale = Vector3.one;
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

                for (int index = 0; index < visibleAnimals.Count; index++)
                {
                    int animal = visibleAnimals[index];

                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    sequence.Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.zero,
                        duration: 0.08f, Ease.Linear, startDelay: 0.08f * index));
                }

                EcsGroup buffer = value.Clone();

                buffer.ExceptWith(visibleAnimals);

                foreach (int animal in buffer)
                {
                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;
                }

                break;
            }

            return sequence;
        }
    }
}