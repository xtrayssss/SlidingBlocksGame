using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class DynamicLayoutSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScrollAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ScrollAspect scrollAspect))
            {
                ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(entity);
                RectTransform content = scrollSnap.ScrollRect.content;
                RectTransform viewport = scrollSnap.ScrollRect.viewport;

                float viewportWidth = viewport.rect.width;
                float elementWidth = scrollSnap.ElementTemplate.rect.width;

                int padding = (int)math.round((viewportWidth - elementWidth) / 2f);

                scrollSnap.LayoutGroup.padding.left = padding;
                scrollSnap.LayoutGroup.padding.right = padding;

                float visiblePartOfSideElements = elementWidth * scrollSnap.VisiblePartRatio;
                float spacing = (viewportWidth - elementWidth - 2 * visiblePartOfSideElements) / 2f;

                scrollSnap.LayoutGroup.spacing = spacing;

                LayoutRebuilder.ForceRebuildLayoutImmediate(content);

#if UNITY_EDITOR

                if (scrollSnap.IsDebug)
                {
                    Debug.Log(
                        $"Viewport Width: {viewportWidth}, Element Width: {elementWidth}, Spacing: {spacing}, Visible Part: {visiblePartOfSideElements}");
                }
#endif
            }
        }
    }
}