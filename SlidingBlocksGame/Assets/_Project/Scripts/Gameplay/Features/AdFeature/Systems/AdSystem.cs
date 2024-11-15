using _Project.Scripts.Gameplay.Features.AdFeature.Components;
using _Project.Scripts.Gameplay.Features.PauseFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.AdFeature.Systems
{
    public class AdSystem : IEcsInit, IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ShowRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ShowAdRequest> ShowAdRequest;
        }

        private class AdAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<AdTag> AdTag;
            [Opt] public readonly EcsTagPool<AdCompletedEvent> AdCompletedEvent;
        }

        private class PauseAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PauseRequest> Pause;
            [Opt] public readonly EcsTagPool<UnpauseRequest> Unpause;
        }

        public void Init()
        {
            YandexGame.CloseFullAdEvent += static () =>
            {
                EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();
                int @event = world.NewEntity();
                world.GetPool<AdCompletedEvent>().Add(@event);
            };
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ShowRequestAspect _))
            {
                if (YandexGame.nowAdsShow ||
                    !(YandexGame.timerShowAd >= YandexGame.Instance.infoYG.fullscreenAdInterval))
                {
                    EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();
                    int @event = world.NewEntity();
                    world.GetPool<AdCompletedEvent>().Add(@event);
                    world.GetPool<AdCompletedProcessedMarker>().Add(@event);
                }
                else
                {
                    YandexGame.FullscreenShow();

                    int ad = _world.NewEntity();
                    AdAspect adAspect = _world.GetAspect<AdAspect>();
                    adAspect.AdTag.Add(ad);

                    int request = _world.NewEntity();
                    PauseAspect pauseAspect = _world.GetAspect<PauseAspect>();
                    pauseAspect.Pause.Add(request);
                }
            }

            foreach (int ad in _world.Where(out AdAspect adAspect))
            {
                if (!YandexGame.nowFullAd)
                {
                    Debug.Log("Unlock");

                    int request = _world.NewEntity();
                    PauseAspect pauseAspect = _world.GetAspect<PauseAspect>();
                    pauseAspect.Unpause.Add(request);
                    adAspect.AdCompletedEvent.Add(ad);
                }
            }
        }
    }
}