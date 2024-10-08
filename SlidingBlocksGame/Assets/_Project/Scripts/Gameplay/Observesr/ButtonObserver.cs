using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Observesr
{
    public class ButtonObserver : MonoBehaviour
    {
        public EntityTemplate EntityCfg;

        public void OnClick()
        {
            Debug.Log("Click");
            int @event = EcsDefaultWorldSingletonProvider.Instance.Get().NewEntity(EntityCfg);
            EcsDefaultWorldSingletonProvider.Instance.Get().GetPool<ButtonClickedEvent>().Add(@event);
        }
    }
}