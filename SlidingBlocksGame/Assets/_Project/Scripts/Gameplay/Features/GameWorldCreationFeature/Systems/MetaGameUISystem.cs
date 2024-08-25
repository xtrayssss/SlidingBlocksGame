using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class MetaGameUISystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class HideMetaGameUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(HideMetaGameUIRequest))]
            [Inc] public readonly EcsPool<MetaGameUI> UI;
        }

        private class ShowMetaGameUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ShowMetaGameUIRequest))]
            [Inc] public readonly EcsPool<MetaGameUI> UI;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ShowMetaGameUIAspect aspect))
            {
                foreach (GameObject ui in aspect.UI.Get(entity).Value)
                {
                    Tween.Scale(ui.transform.transform, Vector3.one, 0.2f, Ease.OutBack);

                    ui.SetActive(true);
                }
            }

            foreach (int entity in _world.Where(out HideMetaGameUIAspect aspect))
            {
                foreach (GameObject ui in aspect.UI.Get(entity).Value)
                {
                    Tween.Scale(ui.transform.transform, Vector3.zero, 0.2f, Ease.Linear)
                        .OnComplete(ui, target =>
                        {
                            target.SetActive(false);
                        });
                }
            }
        }
    }
}