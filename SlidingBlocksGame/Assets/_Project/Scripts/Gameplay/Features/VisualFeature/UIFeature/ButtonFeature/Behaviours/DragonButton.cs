using _Project.Scripts.Gameplay.Templates;
using DCFApixels.DragonECS;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Behaviours
{
    public class DragonButton : Button
    {
        public EntityTemplate EntityCfg;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();
            int @event = world.NewEntity(EntityCfg);
            world.GetPool<ButtonFeature.Components.ButtonClickedEvent>().Add(@event);
        }
    }
}