using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class CleanupLevelSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<CleanupLevelRequest> _;
        }
        private class LifeTimeAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<LevelLifeTimeMarker> _;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out Aspect _))
            {
                foreach (int entity in _world.Where(out LifeTimeAspect _)) 
                    _world.DelEntity(entity);
            }
        }
    }
}