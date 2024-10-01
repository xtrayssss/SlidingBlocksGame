using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class CleanupLevelSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(CleanupLevelRequest))]
            [Opt] public readonly EcsTagPool<LevelClearedEvent> LevelCleared;
        }

        private class LifeTimeAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<LevelLifeTimeMarker> _levelLifeTime;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelAspect levelAspect))
            {
                foreach (int entity in _world.Where(out LifeTimeAspect _))
                    _world.DelEntity(entity);
                
                levelAspect.LevelCleared.Add(level);
            }
        }
    }
}