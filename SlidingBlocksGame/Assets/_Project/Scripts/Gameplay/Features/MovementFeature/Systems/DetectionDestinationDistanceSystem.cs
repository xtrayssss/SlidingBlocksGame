using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class DetectionDestinationDistanceSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class AssignedGroupAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DetectionDistanceRequest))]
            [Inc] public readonly EcsPool<ObstacleCellPositions> ObstacleCellPositions;
            [Inc] public readonly EcsPool<Units> Units;

            [Opt] public readonly EcsTagPool<CalculateObstacleDistanceRequest> CalculateObstacleDistanceRequest;
            [Opt] public readonly EcsTagPool<CalculationMaxDistanceRequest> CalculationMaxDistanceRequest;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out AssignedGroupAspect aspect))
            {
                ref readonly Units units = ref aspect.Units.Read(entity);
                ref readonly ObstacleCellPositions obstacleCellPositions = ref aspect.ObstacleCellPositions.Read(entity);

                Debug.Log("123");
                
                if (obstacleCellPositions.Value.Count != 0)
                {
                    Debug.Log("123");
                    foreach (entlong unit in units.Value.Longs)
                    {
                        if (unit.TryGetID(out int unitID))
                        {
                            aspect.CalculateObstacleDistanceRequest.TryAdd(unitID);
                        }
                    }
                }
                else
                {
                    Debug.Log("321");

                    foreach (entlong unit in units.Value.Longs)
                    {
                        if (unit.TryGetID(out int unitID))
                        {
                            aspect.CalculationMaxDistanceRequest.TryAdd(unitID);
                        }
                    }
                }
            }
        }
    }
}