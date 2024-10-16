using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Extensions
{
    public static class UIExtensions
    {
        private class UIAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UIElement> UIElements;
        }

        public static entlong NewUIEntity(this EcsWorld world, EcsEntityConnect connect)
        {
            entlong element = world.NewEntityLong();
            connect.Connect(element, applyTemplates: true);

            UIAspect uiAspect = world.GetAspect<UIAspect>();

            ref UIElement uiElement = ref uiAspect.UIElements.Add(element.ID);
            uiElement.RectTransform = connect.transform as RectTransform;

            return element;
        }

        public static void ConnectUI(this EcsEntityConnect source, entlong entity)
        {
            EcsWorld world = entity.World;
            
            source.Connect(entity, applyTemplates: true);
        
            UIAspect uiAspect = world.GetAspect<UIAspect>();
            
            ref UIElement uiElement = ref uiAspect.UIElements.Add(entity.ID);
            uiElement.RectTransform = source.transform as RectTransform;
        }
    }
}