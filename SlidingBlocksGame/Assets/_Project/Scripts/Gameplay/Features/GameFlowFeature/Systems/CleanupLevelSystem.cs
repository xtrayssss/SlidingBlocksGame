using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
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