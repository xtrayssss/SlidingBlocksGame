using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayBestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class BestAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(BestTag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;

            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;

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
                    Tween.PunchScale(aspect.RectTransforms.Read(entity).Value, new Vector3(0.5f, 0.5f), 0.2f);
                }
            }
        }
    }
}