using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

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

            [Inc] public readonly EcsPool<InGamePurchaseAnimals> PurchaseAnimals;
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

                    scrollSnap.Value.IsOffEffects = true;

                    scrollSnap.Sequence.Stop();
                    
                    // TODO: remove closure allocation
                    
                    scrollSnap.Sequence = Sequence.Create()
                        .Chain(AnimateScrollElements(animalsShopWindowAspect.PurchaseAnimals.Read(window).Value))
                        .Insert(0.3f,
                            Tween.Scale(gameObjectConnect.Connect.transform, Vector3.zero, 0.15f, Ease.InOutSine))
                        .ChainCallback(
                            () =>
                            {
                                _world.GetPool<ScrollSnapRef>().Get(window).Value.IsOffEffects = false;
                                _world.GetPool<GameObjectConnect>().Get(window).Connect.gameObject.SetActive(false);
                            });
                }
            }
        }

        private Sequence AnimateScrollElements(EcsGroup value)
        {
            Sequence sequence = Sequence.Create();

            for (int i = 0; i < value.Count; i++)
            {
                ref GameObjectConnect gameObjectConnect = ref _world.GetPool<GameObjectConnect>().Get(value[i]);

                sequence.Group(Sequence.Create()
                    .Group(Tween.Scale(target: gameObjectConnect.Connect.transform, endValue: Vector3.zero,
                        duration: 0.5f,
                        Ease.OutBack, startDelay: 0.1f * i)));
            }

            return sequence;
        }
    }
}