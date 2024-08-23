using System.Linq.Expressions;
using System.Threading.Tasks;
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

                    scrollSnap.Sequence.Stop();

                    gameObjectConnect.Connect.transform.localScale = Vector3.zero;

                    // TODO: remove closure allocation
                    
                    ScrollSnapRef scrollSnapCopy = scrollSnap;

                    scrollSnap.Sequence = Sequence.Create()
                        .Group(Tween.Scale(gameObjectConnect.Connect.transform, Vector3.one, 0.2f, Ease.InOutSine))
                        .ChainCallback(() =>
                        {
                            scrollSnapCopy.Value.ScrollRect.enabled = true;
                            scrollSnapCopy.Value.IsApplyEffects = true;
                        })
                        .Chain(AnimateScrollElements(animalsShopWindowAspect.PurchaseAnimals.Read(window).Entities));

                    scrollSnapCopy.Value.gameObject.SetActive(true);
                }
            }
        }

        private Sequence AnimateScrollElements(EcsGroup value)
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
}