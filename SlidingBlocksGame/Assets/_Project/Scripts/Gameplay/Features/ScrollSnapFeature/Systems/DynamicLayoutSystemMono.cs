using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class DynamicLayoutSystemMono : MonoBehaviour
    {
        public ScrollRect _scrollRect;
        public RectTransform ElementTemplate;
        public HorizontalLayoutGroup HorizontalLayoutGroup;
        [SerializeField] private float VisiblePartRatio;

        public void Update()
        {
                RectTransform content = _scrollRect.content;
                RectTransform viewport = _scrollRect.viewport;

                float viewportWidth = viewport.rect.width;
                float elementWidth = ElementTemplate.rect.width;

                int padding = (int)math.round((viewportWidth - elementWidth) / 2f);

                HorizontalLayoutGroup.padding.left = padding;
                HorizontalLayoutGroup.padding.right = padding;

                float visiblePartOfSideElements = elementWidth *VisiblePartRatio;
                float spacing = (viewportWidth - elementWidth - 2 * visiblePartOfSideElements) / 2f;

                HorizontalLayoutGroup.spacing = spacing;

                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
    }
}