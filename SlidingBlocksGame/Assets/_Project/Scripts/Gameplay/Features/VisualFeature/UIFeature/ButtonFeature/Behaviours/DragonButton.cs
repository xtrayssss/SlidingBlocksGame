using _Project.Scripts.Gameplay.Templates;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Behaviours
{
    public class DragonButton : Button
    {
        public EntityTemplate EntityCfg;
        private UnityAction _click;

        protected override void OnEnable()
        {
            base.OnEnable();

            _click = () =>
            {
                Debug.Log("CLICK");
                EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();
                int @event = world.NewEntity(EntityCfg);
                world.GetPool<ButtonFeature.Components.ButtonClickedEvent>().Add(@event);
            };

            onClick.AddListener(_click);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            onClick.RemoveListener(_click);
        }
    }
}