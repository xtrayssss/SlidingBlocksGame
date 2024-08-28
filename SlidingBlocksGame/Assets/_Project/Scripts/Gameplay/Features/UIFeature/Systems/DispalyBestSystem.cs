using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DispalyBestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class BestAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(BestTag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;

            [Inc] public readonly EcsPool<BestCounter> BestCounters;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<LevelTag> _createLevelRequests;
            [Inc] private readonly EcsTagPool<SpawnedEvent> _spawnedEvents;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out LevelAspect _))
            {
                foreach (int entity in _world.Where(out BestAspect aspect))
                {
                    aspect.Texts.Get(entity).Value.text = (++aspect.BestCounters.Get(entity).Value).ToString();
                }
            }
        }
    }
}