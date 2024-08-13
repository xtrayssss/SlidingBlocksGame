using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Observesr
{
    public class ButtonObserver : MonoBehaviour
    {
        public TemporaryEntityTemplate EntityCfg;

        public void OnClick()
        {
            int @event = EcsDefaultWorldSingletonProvider.Instance.Get().NewEntity(EntityCfg);
            EcsDefaultWorldSingletonProvider.Instance.Get().GetPool<ButtonClickedEvent>().Add(@event);
        }
    }
}