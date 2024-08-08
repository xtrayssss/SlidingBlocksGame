using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEditor;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class DestroyUnitRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Obstacle> Obstacles;
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> OccupancyMarker;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Opt] public readonly EcsTagPool<DestroyUnitRequest> DestroyUnitRequest;
        }

        private class ObstacleAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> OccupancyMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out AnimalAspect animalAspect))
            {
                ref readonly Obstacle obstacle = ref animalAspect.Obstacles.Get(entity);

                if (obstacle.Value.TryGetID(out int obstacleID))
                {
                    var obstacleAspect = _world.GetAspect<ObstacleAspect>();

                    if (obstacleAspect.IsMatches(obstacleID))
                    {
                        animalAspect.DestroyUnitRequest.Add(entity);
                    }
                }
            }
        }
    }
}