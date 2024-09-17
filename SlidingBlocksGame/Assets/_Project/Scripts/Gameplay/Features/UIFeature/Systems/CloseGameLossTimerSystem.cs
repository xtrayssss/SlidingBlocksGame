using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class CloseGameLossTimerSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameLossTimerTag))]
            [IncImplicit(typeof(CloseGameLossTimerRequest))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            
            [Opt] public readonly EcsTagPool<GameLossTimerClosedEvent> ClosedEvent;
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