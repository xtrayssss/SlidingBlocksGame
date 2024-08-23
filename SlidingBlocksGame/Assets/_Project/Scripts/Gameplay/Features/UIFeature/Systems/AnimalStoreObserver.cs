using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class AnimalStoreObserver : MonoBehaviour
    {
        public void Tween()
        {
            ScrollSnap scrollSnap = GetComponentInParent<ScrollSnap>();

            var sequence = Sequence.Create();
            
            for (var index = 0; index < scrollSnap.Items.Count / 2 ; index++)
            {
                var item = scrollSnap.Items[index];

                item.localScale = Vector3.zero;
                sequence.Group(PrimeTween.Tween.Scale(item, Vector3.one, 0.3f));
            }
            
            for (int index = scrollSnap.Items.Count / 2; index < scrollSnap.Items.Count ; index++)
            {
                var item = scrollSnap.Items[index];
                item.localScale = Vector3.zero;

                sequence.Chain(PrimeTween.Tween.Scale(item, Vector3.one, 0.3f));
            }
        }
        // public void OnSelected(RectTransform rect, int index)
        // {
        //     Debug.Log("Selected");
        //     EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();
        //
        //     int @event = world.NewEntity();
        //
        //     world.GetPool<AnimalSelectedEvent>().Add(@event);
        //     world.GetPool<AnimalSelectedIndex>().Add(@event).Value = index;
        // }
        //
        // public void OnDeselected(RectTransform rect, int index)
        // {
        //     Debug.Log("DeSelected");
        //
        //     EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();
        //
        //     int @event = world.NewEntity();
        //
        //     world.GetPool<AnimalDeselectedEvent>().Add(@event);
        //     world.GetPool<AnimalSelectedIndex>().Add(@event).Value = index;
        // }

        public void OnSnapped(int index, RectTransform rect)
        {
            Debug.Log("Snapped");

            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            int @event = world.NewEntity();

            world.GetPool<ScrollSnappedEvent>().Add(@event);
            world.GetPool<AnimalSelectedIndex>().Add(@event).Value = index;
            world.GetPool<DeleteEntityCommand>().Add(@event);
        }

        public void OnStarted(int index, RectTransform rect)
        {
            Debug.Log("Started");

            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            int @event = world.NewEntity();

            world.GetPool<ScrollStartedEvent>().Add(@event);
            world.GetPool<AnimalSelectedIndex>().Add(@event).Value = index;
            world.GetPool<DeleteEntityCommand>().Add(@event);
        }
    }
}