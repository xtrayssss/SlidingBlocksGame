using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Systems
{
    public class CloseGameOverTimerSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameOverTimerTag))]
            [IncImplicit(typeof(CloseGameOverTimerRequest))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            
            [Opt] public readonly EcsTagPool<GameOverTimerClosedEvent> ClosedEvent;
            [Opt] public readonly EcsTagPool<ClosedMarker> ClosedMarker;
        }
        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                GameObjectConnect goConnect = aspect.GameObjectConnects.Read(entity);

                Tween
                    .Scale(
                        target: goConnect.Connect.transform,
                        endValue: Vector3.zero,
                        duration: 0.2f,
                        ease: Ease.InBack)
                    .OnComplete(
                        target: goConnect.Connect,
                        onComplete: connect =>
                        {
                            if (!connect.Entity.TryGetID(out int id))
                                return;

                            aspect.ClosedEvent.Add(id);
                            aspect.ClosedMarker.Add(id);
                            connect.gameObject.SetActive(false);
                        });
            }
        }
    }
}