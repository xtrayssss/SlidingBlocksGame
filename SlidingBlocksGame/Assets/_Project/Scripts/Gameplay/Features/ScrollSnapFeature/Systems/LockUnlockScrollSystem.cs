using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class LockUnlockScrollSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class LockScrollAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LockScrollSnapRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;
            
            [Inc] public readonly EcsTagPool<ScrollUnlockedMarker> ScrollUnlockedMarker;
            [Exc] public readonly EcsTagPool<ScrollLockedMarker> ScrollLockedMarker;
        }

        private class UnlockScrollAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(UnlockScrollSnapRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;
        
            [Exc] public readonly EcsTagPool<ScrollUnlockedMarker> ScrollUnlockedMarker;
            [Inc] public readonly EcsTagPool<ScrollLockedMarker> ScrollLockedMarker;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out LockScrollAspect lockScrollAspect))
            {
                ref ScrollSnap scrollSnap = ref lockScrollAspect.ScrollSnap.Get(scroll);
                scrollSnap.ScrollRect.enabled = false;
                
                lockScrollAspect.ScrollLockedMarker.Add(scroll);
                lockScrollAspect.ScrollUnlockedMarker.Del(scroll);
            }

            foreach (int scroll in _world.Where(out UnlockScrollAspect unlockScrollAspect))
            {
                ref ScrollSnap scrollSnap = ref unlockScrollAspect.ScrollSnap.Get(scroll);
                scrollSnap.ScrollRect.enabled = true;

                unlockScrollAspect.ScrollUnlockedMarker.Add(scroll);
                unlockScrollAspect.ScrollLockedMarker.Del(scroll);
            }
        }
    }
}