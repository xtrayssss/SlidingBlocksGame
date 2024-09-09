using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class MetaGameUISystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ShowMetaGameUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ShowMetaGameUIRequest))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<MetaGameUI> UI;
            [Inc] public readonly EcsTagPool<MetaGameUIHiddenMarker> MetaGameUIHiddenMarker;
        }

        private class HideMetaGameUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(HideMetaGameUIRequest))]
            [Inc] public readonly EcsPool<MetaGameUI> UI;

            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Exc] public readonly EcsTagPool<MetaGameUIHiddenMarker> MetaGameUIHiddenMarker;
            [Opt] public readonly EcsTagPool<MetaGameUIHiddenEvent> MetaGameUIHiddenEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ShowMetaGameUIAspect aspect))
            {
                Sequence sequence = Sequence.Create();

                aspect.MetaGameUIHiddenMarker.Del(entity);

                foreach (GameObject ui in aspect.UI.Get(entity).Value)
                {
                    sequence.Group(Tween
                        .Scale(ui.transform.transform, Vector3.one, 0.2f, Ease.OutBack));

                    ui.SetActive(true);
                }
            }

            foreach (int entity in _world.Where(out HideMetaGameUIAspect aspect))
            {
                Sequence sequence = Sequence.Create();

                foreach (GameObject ui in aspect.UI.Get(entity).Value)
                {
                    sequence.Group(Tween.Scale(ui.transform.transform, Vector3.zero, 0.2f, Ease.InBack))
                        .ChainCallback(
                            target: ui,
                            callback: go => go.SetActive(false));
                }

                sequence.ChainCallback(
                    target: aspect.GameObjectConnects.Get(entity).Connect,
                    callback: connect =>
                    {
                        if (!connect.Entity.TryGetID(out int id))
                            return;

                        aspect.MetaGameUIHiddenEvent.Add(id);
                        aspect.MetaGameUIHiddenMarker.Add(id);
                    });
            }
        }
    }
}