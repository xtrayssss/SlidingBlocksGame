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
        }

        private class UnlockScrollAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(UnlockScrollSnapRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out LockScrollAspect lockScrollAspect))
            {
                ref ScrollSnap scrollSnap = ref lockScrollAspect.ScrollSnap.Get(scroll);
                scrollSnap.ScrollRect.enabled = false;
            }

            foreach (int scroll in _world.Where(out UnlockScrollAspect unlockScrollAspect))
            {
                ref ScrollSnap scrollSnap = ref unlockScrollAspect.ScrollSnap.Get(scroll);
                scrollSnap.ScrollRect.enabled = true;
            }
        }
    }
}