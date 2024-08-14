using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class BestVisualizeSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(BestTag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;

            [Inc] public readonly EcsPool<BestCounter> BestCounters;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CreateLevelRequest> _;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out LevelAspect _))
            {
                foreach (int entity in _world.Where(out Aspect aspect))
                {
                    aspect.Texts.Get(entity).Value.text = (++aspect.BestCounters.Get(entity).Value).ToString();
                }
            }
        }
    }
}