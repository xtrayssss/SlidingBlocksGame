using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
    {
        public class ScrollIdleSystem : IEcsRun
        {
            [EcsInject] private EcsDefaultWorld _world;

            private class ScrollAspect : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<IdleState> IdleStates;
                [Opt] public readonly EcsTagPool<DraggingState> DraggingState;
            }

            public void Run()
            {
                foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        scrollAspect.IdleStates.Del(scroll);

                        scrollAspect.DraggingState.Add(scroll);
                    }
                }
            }
        }
    }
}