using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class SettingsMenuSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class OpenMenuAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _;
            [Inc] public readonly EcsTagPool<SettingsButtonTag> _1;
        }

        private class CloseMenuAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _;
            [Inc] public readonly EcsTagPool<CloseSettingsButtonTag> _1;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Inc] public readonly EcsPool<SettingsMenuView> SettingsMenuViews;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out OpenMenuAspect aspect))
            {
                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly var settingsMenuView = ref gameScreenAspect.SettingsMenuViews.Read(gameScreen);

                    settingsMenuView.Value.transform.localScale = Vector3.zero;

                    settingsMenuView.Value.gameObject.SetActive(true);

                    Sequence.Create(cycleMode: CycleMode.Yoyo)
                        .Chain(Tween.Scale(settingsMenuView.Value.transform, Vector3.one * 1.2f, 0.1f, Ease.OutQuad)
                            .Chain(Tween.Scale(settingsMenuView.Value.transform, Vector3.one * 1f, 0.05f,
                                Ease.InQuad)));
                }
            }

            foreach (int entity in _world.Where(out CloseMenuAspect aspect))
            {
                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly var settingsMenuView = ref gameScreenAspect.SettingsMenuViews.Read(gameScreen);

                    SettingsMenuView view = settingsMenuView;
                    
                    Sequence.Create(cycleMode: CycleMode.Yoyo)
                        .Chain(Tween.Scale(settingsMenuView.Value.transform, Vector3.one * 1.2f, 0.1f, Ease.OutQuad)
                            .Chain(Tween.Scale(settingsMenuView.Value.transform, Vector3.zero, 0.05f,
                                Ease.InQuad))).ChainCallback(() => view.Value.gameObject.SetActive(false));
                }
            }
        }
    }
}