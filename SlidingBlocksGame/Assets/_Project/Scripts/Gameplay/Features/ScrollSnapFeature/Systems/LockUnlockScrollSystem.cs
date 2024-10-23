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

            [Inc] public readonly EcsTagPool<UnlockedMarker> UnlockedMarker;
            [Exc] public readonly EcsTagPool<LockedMarker> LockedMarker;
        }

        private class UnlockScrollAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(UnlockScrollSnapRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;

            [Exc] public readonly EcsTagPool<UnlockedMarker> UnlockedMarker;
            [Inc] public readonly EcsTagPool<LockedMarker> LockedMarker;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out LockScrollAspect lockScrollAspect))
            {
                ref ScrollSnap scrollSnap = ref lockScrollAspect.ScrollSnap.Get(scroll);
                scrollSnap.ScrollRect.enabled = false;

                lockScrollAspect.LockedMarker.Add(scroll);
                lockScrollAspect.UnlockedMarker.Del(scroll);

                foreach (entlong item in scrollSnap.Items.Longs)
                {
                    if (item.TryGetID(out int itemID))
                    {
                        lockScrollAspect.LockedMarker.Add(itemID);
                        lockScrollAspect.UnlockedMarker.Del(itemID);
                    }
                }
            }

            foreach (int scroll in _world.Where(out UnlockScrollAspect unlockScrollAspect))
            {
                ref ScrollSnap scrollSnap = ref unlockScrollAspect.ScrollSnap.Get(scroll);
                scrollSnap.ScrollRect.enabled = true;

                unlockScrollAspect.UnlockedMarker.Add(scroll);
                unlockScrollAspect.LockedMarker.Del(scroll);
                
                foreach (entlong item in scrollSnap.Items.Longs)
                {
                    if (item.TryGetID(out int itemID))
                    {
                        unlockScrollAspect.UnlockedMarker.Add(itemID);
                        unlockScrollAspect.LockedMarker.Del(itemID);
                    }
                }
            }
        }
    }
}