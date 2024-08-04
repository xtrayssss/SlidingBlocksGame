using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Playables;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class DestroySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Obstacle> Obstacles;
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> OccupancyMarker;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        private class ObstacleAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> OccupancyMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly Obstacle obstacle = ref aspect.Obstacles.Get(entity);

                if (obstacle.Value.TryGetID(out int obstacleID))
                {
                    var obstacleAspect = _world.GetAspect<ObstacleAspect>();

                    if (obstacleAspect.IsMatches(obstacleID))
                    {
                        Object.Destroy(aspect.GameObjectConnects.Read(entity).Connect.gameObject);
                    }
                }
            }
        }
    }
}