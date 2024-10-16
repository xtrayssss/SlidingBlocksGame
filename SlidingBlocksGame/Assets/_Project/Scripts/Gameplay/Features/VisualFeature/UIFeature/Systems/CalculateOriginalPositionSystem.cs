using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems
{
    public class CalculateOriginalPositionSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class UIElementAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateOriginalPositionRequest))]
            [Inc] public readonly EcsPool<UIElement> UIElements;
            [Opt] public readonly EcsPool<OriginalAnchoredPosition> OriginalPositions;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out UIElementAspect uiElementAspect))
            {
                ref UIElement uiElement = ref uiElementAspect.UIElements.Get(entity);
                ref OriginalAnchoredPosition anchoredPosition = ref uiElementAspect.OriginalPositions.Add(entity);

                anchoredPosition.Value = uiElement.RectTransform.anchoredPosition;
            }
        }
    }
}