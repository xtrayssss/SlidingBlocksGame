using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

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
            [Inc] public readonly EcsPool<ScrollSnapRef> ScrollSnap;
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

                    _world.GetPool<ClosedMarker>().Del(window);

                    Debug.Log("123");

                    scrollSnap.Sequence.Stop();

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                    // TODO: remove closure allocation
                    ScrollSnapRef scrollSnapCopy = scrollSnap;

                    scrollSnap.Sequence = Sequence.Create()
                        .Group(Tween.Scale(gameObjectConnect.Connect.transform, Vector3.one, 0.2f, Ease.InOutSine))
                        .ChainCallback(() => { Debug.Log("Result"); })
                        .Chain(AnimateScrollElements(animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities,
                            scrollSnap.Value)).ChainCallback(() => { Debug.Log("Result"); });

                    scrollSnapCopy.Value.gameObject.SetActive(true);
                }
            }
        }

        private Sequence AnimateScrollElements(EcsGroup value, ScrollSnap scrollSnap)
        {
            Sequence sequence = Sequence.Create();

            for (int i = 0; i < value.Count; i++)
            {
                EcsSpan visibleAnimals = default;

                if (!_world.GetPool<ScrollSnappedMarker>().Has(value[i]))
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

                Sequence visibleSequence = Sequence.Create();
                
                for (int index = 0; index < visibleAnimals.Count; index++)
                {
                    int animal = visibleAnimals[index];

                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                    visibleSequence.Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.one,
                        duration: 0.08f, Ease.Linear, startDelay: 0.08f * index));
                }

                visibleSequence.ChainCallback(() =>
                {
                    scrollSnap.ScrollRect.enabled = true;
                    scrollSnap.IsApplyEffects = true;
                });
                
                sequence.Group(visibleSequence);

                EcsGroup buffer = value.Clone();

                buffer.ExceptWith(visibleAnimals);

                for (int index = 0; index < buffer.Count; index++)
                {
                    int animal = buffer[index];

                    ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(animal);

                    gameObjectConnect.Connect.transform.localScale = Vector3.one;
                }
            }

            return sequence;
        }
    }
}