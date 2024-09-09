using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class CleanupLevelSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [IncImplicit(typeof(CleanupLevelRequest))]
            [Opt] public readonly EcsTagPool<LevelClearedEvent> LevelCleared;
        }

        private class LifeTimeAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<LevelLifeTimeMarker> _levelLifeTime;
        }

        public void Run()
        {
            foreach (int game in _world.Where(out GameAspect gameAspect))
            {
                foreach (int entity in _world.Where(out LifeTimeAspect _))
                    _world.DelEntity(entity);
                
                gameAspect.LevelCleared.Add(game);
            }
        }
    }
}