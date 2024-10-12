using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class ScrollSnapSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScrollAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
            {
                ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(scroll);

                scrollSnap.ScrollPosition = scrollSnap.ScrollRect.horizontalScrollbar.value;
                
                UpdateNearest(ref scrollSnap);

                scrollSnap.TargetIndex = scrollSnap.NearestIndex;
                scrollSnap.TargetPosition = scrollSnap.Positions[scrollSnap.NearestIndex];
            }
        }

        private void UpdateNearest(ref ScrollSnap scrollSnap)
        {
            for (int index = 0; index < scrollSnap.ItemCount; index++)
            {
                float distance = math.abs(scrollSnap.ScrollPosition - scrollSnap.Positions[index]);
                
                if (distance <= scrollSnap.Distance / 2) 
                    scrollSnap.NearestIndex = index;
            }
        }
    }
}